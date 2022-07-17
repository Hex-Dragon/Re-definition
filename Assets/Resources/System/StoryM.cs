using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Linq;

public class StoryM : MonoBehaviour {

    // 文本
    public TMPro.TextMeshProUGUI textStory;
    public RectTransform shapeBack;
    /// <summary>
    /// 设置旁白，并返回预期时间。旁白字幕将在预期时间后消失。
    /// </summary>
    public float SetStoryText(string textCn, string textEn = "English") {
        bool isCn = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)";
        textStory.text = isCn ? textCn : textEn;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textStory.rectTransform);
        // 动画
        textStory.color = Color.black;
        DOTween.Kill("StoryFont"); textStory.DOColor(Color.white, 0.25f).SetId("StoryFont");
        DOTween.Kill("StoryBackground"); shapeBack.DOScaleX((textStory.rectTransform.rect.width + 40f) / 100f, 0.15f).SetId("StoryBackground");
        // 消失
        float time = 1.5f + (isCn ? textCn.Length / 3f : textEn.Split(" ").Length / 3f);
        StopCoroutine("WaitForHideStoryText"); StartCoroutine("WaitForHideStoryText", time);
        Debug.Log(time + " -> " + textCn);
        return time;
    }
    IEnumerator WaitForStoryText(string textCh, string textEn = "English") {
        yield return new WaitForSeconds(SetStoryText(textCh, textEn));
    }
    IEnumerator WaitForHideStoryText(float delay) {
        yield return new WaitForSeconds(delay);
        textStory.text = "";
        Debug.Log(delay);
        DOTween.Kill("StoryBackground"); shapeBack.DOScaleX(0, 0.2f).SetId("StoryBackground");
    }

    // 刷怪
    private Dictionary<Spawner.EnemyType, List<Spawner>> spawners = null;
    public void Spawn(Spawner.EnemyType type, int count = 1) {
        // 初始化
        if (spawners == null) {
            spawners = new();
            foreach (Spawner spawner in FindObjectsOfType<Spawner>()) {
                if (spawners.ContainsKey(spawner.type)) {
                    spawners[spawner.type].Add(spawner);
                } else {
                    spawners[spawner.type] = new() { spawner };
                }
            }
        }
        // 实际刷怪
        spawners[type].Shuffle();
        for (int i = 0; i < count; i++) {
            if (i >= spawners[type].Count) i = 0;
            spawners[type][i].Spawn();
        }
    }

    // 流程控制
    IEnumerator WaitUntilClear(int remain = 0) {
        while (FindObjectsOfType<EnemyBase>().Length - FindObjectsOfType<EnemyArrow>().Length > remain) { // 要算上自己
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator WaitUntilPickDice() {
        while (FindObjectsOfType<DiceEntity>().Length > 0) {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 故事
    public int stage = 0;
    void Start() {
        StartCoroutine(test());
        StartCoroutine(spawnArrow());
    }
    bool canSpawnArrow = true;
    IEnumerator test() {
        while (true) {
            canSpawnArrow = true;
            Spawn(Spawner.EnemyType.Mover, 3);
            SetStoryText("先刷几个瞎晃悠的", "");
            yield return new WaitForSeconds(3f);

            Spawn(Spawner.EnemyType.Shooter);
            SetStoryText("炮塔！", "");
            yield return new WaitForSeconds(2f);

            Spawn(Spawner.EnemyType.Heavy, 2);
            SetStoryText("最后是两个贼硬的……", "");
            yield return StartCoroutine(WaitUntilClear(2));

            canSpawnArrow = false;
            InputM.DropDice(Modules.RandomOne(InputM.keyTypes.ToList()));
            SetStoryText("来一个骰子吧……？", "");
            yield return StartCoroutine(WaitUntilPickDice());

            yield return StartCoroutine(WaitForStoryText("准备好，下一轮要来了！", ""));
        }
    }
    IEnumerator spawnArrow() {
        while (true) {
            yield return new WaitForSeconds(5f);
            if (canSpawnArrow) Spawn(Spawner.EnemyType.Arrow, 2);
        }
    }

}
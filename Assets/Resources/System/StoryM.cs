using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Linq;

public class StoryM : MonoBehaviour {

    // �ı�
    public TMPro.TextMeshProUGUI textStory;
    public RectTransform shapeBack;
    /// <summary>
    /// �����԰ף�������Ԥ��ʱ�䡣�԰���Ļ����Ԥ��ʱ�����ʧ��
    /// </summary>
    public float SetStoryText(string textCn, string textEn = "English") {
        bool isCn = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)";
        textStory.text = isCn ? textCn : textEn;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textStory.rectTransform);
        // ����
        textStory.color = Color.black;
        DOTween.Kill("StoryFont"); textStory.DOColor(Color.white, 0.25f).SetId("StoryFont");
        DOTween.Kill("StoryBackground"); shapeBack.DOScaleX((textStory.rectTransform.rect.width + 40f) / 100f, 0.15f).SetId("StoryBackground");
        // ��ʧ
        float time = 1.5f + (isCn ? textCn.Length / 1.5f : textEn.Split(" ").Length / 2f);
        StopCoroutine(nameof(WaitForHideStoryText)); StartCoroutine(WaitForHideStoryText(time));
        return time;
    }
    IEnumerator WaitForStoryText(string textCh, string textEn = "English") {
        yield return new WaitForSeconds(SetStoryText(textCh, textEn));
    }
    IEnumerator WaitForHideStoryText(float delay) {
        yield return new WaitForSeconds(delay);
        textStory.text = "";
        DOTween.Kill("StoryBackground"); shapeBack.DOScaleX(0, 0.2f).SetId("StoryBackground");
    }

    // ˢ��
    private Dictionary<Spawner.EnemyType, List<Spawner>> spawners = null;
    public void Spawn(Spawner.EnemyType type, int count = 1) {
        // ��ʼ��
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
        // ʵ��ˢ��
        spawners[type].Shuffle();
        for (int i = 0; i < Mathf.Min(6, count); i++) {
            spawners[type][i].Spawn();
        }
    }

    // ���̿���
    IEnumerator WaitUntilClear(int remain = 0) {
        while (FindObjectsOfType<EnemyBase>().Length - FindObjectsOfType<EnemyArrow>().Length > remain) { // Ҫ�����Լ�
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator WaitUntilPickDice() {
        while (FindObjectsOfType<DiceEntity>().Length > 0) {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // ����
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
            SetStoryText("��ˢ����Ϲ���Ƶ�", "");
            yield return new WaitForSeconds(4f);

            Spawn(Spawner.EnemyType.Heavy, 2);
            SetStoryText("Ȼ����������Ӳ�ġ���", "");
            yield return StartCoroutine(WaitUntilClear(2));

            canSpawnArrow = false;
            InputM.DropDice(Modules.RandomOne(InputM.keyTypes.ToList()));
            SetStoryText("��һ�����Ӱɡ�����", "");
            yield return StartCoroutine(WaitUntilPickDice());

            yield return StartCoroutine(WaitForStoryText("׼���ã���һ��Ҫ���ˣ�", ""));
        }
    }
    IEnumerator spawnArrow() {
        while (true) {
            yield return new WaitForSeconds(5f);
            if (canSpawnArrow) Spawn(Spawner.EnemyType.Arrow, 2);
        }
    }

}

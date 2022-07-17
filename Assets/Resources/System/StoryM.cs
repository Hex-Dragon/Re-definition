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
        float time = 1f + (isCn ? textCn.Length / 4f : textEn.Split(" ").Length / 4f);
        StopCoroutine("WaitForHideStoryText"); StartCoroutine("WaitForHideStoryText", time);
        Debug.Log(time + " -> " + textCn);
        return time;
    }
    IEnumerator WaitForStoryText(string textCh, string textEn = "English") {
        yield return new WaitForSeconds(SetStoryText(textCh, textEn));
    }
    IEnumerator WaitForHideStoryText(float delay) {
        yield return new WaitForSeconds(delay);
        Debug.Log("隐藏：" + delay);
        HideStoryText();
    }
    void HideStoryText() {
        StopCoroutine("WaitForHideStoryText");
        textStory.text = "";
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
    IEnumerator WaitUntilClear(int remain, float waitTime = -1, string textCn = "", string textEn = "") {
        while (GetEnemyCount() > remain && !respawning) {
            if (waitTime >= 0f && waitTime < 0.3f) SetStoryText(textCn, textEn);
            waitTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
    }
    int GetEnemyCount() => FindObjectsOfType<EnemyBase>().Length - FindObjectsOfType<EnemyArrow>().Length;
    IEnumerator WaitUntilPickDice(float waitTime = -1, string textCn = "", string textEn = "") {
        while (GetDiceCount() > 0 && !respawning) {
            if (waitTime >= 0f && waitTime < 0.3f) SetStoryText(textCn, textEn);
            waitTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
    }
    int GetDiceCount() => FindObjectsOfType<DiceEntity>().Count(dice => !dice.pickedUp);

    // 故事
    public int stage = 0; private bool stageCompleted = true;
    public Image panDeath;
    void Start() {
        StartCoroutine("ArrowSpawner");
        StartCoroutine("Main");
    }
    IEnumerator Main() {
        while (true) {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Q)) {
                SetStoryText("已跳过阶段：" + stage, "Skip Stage: " + stage);
                stageCompleted = true;
            }
            if (Player.hp <= 0) {
                stageCompleted = false;
                yield return StartCoroutine("Respawn");
            }
            if (stageCompleted) {
                deathInStage = 0;
                stageCompleted = false;
                StopCoroutine("Stage" + stage);
                stage++;
                Debug.Log("已到达阶段：" + stage);
                StartCoroutine("Stage" + stage);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    private int deathInStage = 0; private bool respawning = false, noDifficulty = false;
    IEnumerator Respawn() {
        deathInStage++; respawning = true;
        // 停止场景
        arrowMax = 0;
        Player.instance.transform.localScale = Vector3.zero;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.enabled = false;
        StopCoroutine("Stage" + stage);
        StopCoroutine("HideStoryText");
        yield return new WaitForSeconds(1.5f);
        if (!noDifficulty) {
            _ = Modules.randomDefault.Next(0, 3) switch {
                0 => SetStoryText("完了，寄咯！", ""),
                1 => SetStoryText("哦吼，下次加油吧", ""),
                2 => SetStoryText("诶，怎么回事？", ""),
                3 => SetStoryText("还好没开极限模式……", ""),
                _ => throw new System.NotImplementedException(),
            };
        } else {
            _ = Modules.randomDefault.Next(0, 2) switch {
                0 => SetStoryText("不是吧？这也能死？", ""),
                1 => SetStoryText("你是特么故意的吧！", ""),
                2 => SetStoryText("哈？", ""),
                _ => throw new System.NotImplementedException(),
            };
        }
        yield return new WaitForSeconds(0.5f);
        // 转为黑屏
        panDeath.DOFade(1, 0.3f); panDeath.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOFade(1, 1f);
        yield return new WaitForSeconds(0.3f);
        // 复原
        FindObjectsOfType<EnemyBase>().ToList().ForEach(e => GameObject.Destroy(e.gameObject, 1f));
        Player.hp = 3;
        Player.instance.currentBullet = Player.instance.maxBullet;
        yield return new WaitForSeconds(3f);
        // 淡出黑屏，玩家重生
        respawning = false;
        panDeath.DOFade(0, 0.8f); panDeath.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOFade(0, 0.3f);
        Player.instance.enabled = true;
        Player.instance.transform.localScale = Vector3.one;
        Player.instance.transform.position = new Vector3(-6, 10, 0);
        // 继续游戏
        yield return new WaitForSeconds(2f);
        StartCoroutine("Stage" + stage);
    }
    IEnumerator Stage1() {
        arrowMax = 0; noDifficulty = false;
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("哦，哦！你好，欢迎欢迎", ""));
        yield return StartCoroutine(WaitForStoryText("先来过个教程啥的吧", ""));
        yield return StartCoroutine(WaitForStoryText("游戏的按键操作写在了右上角，应该是个人都看得懂", ""));
        SetStoryText("你不如先拿它们俩试试按键吧！", "");
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitUntilClear(0, 10f, "快点把它干掉，然后才好继续……", ""));
        stageCompleted = true;
    }
    IEnumerator Stage2() {
        arrowMax = 1; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("哦，教程最后一条……", ""));
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("弹药不够的时候记得换弹", ""));
        Spawn(Spawner.EnemyType.Shooter, 1);
        SetStoryText("加油，教程结束了！", "");
        yield return StartCoroutine(WaitUntilClear(1, 10f, "仔细一看，这个角色的手上连枪都没有……", ""));
        if (Player.hp < 3) {
            Player.hp = 3;
            Spawn(Spawner.EnemyType.Mover, 1);
            yield return StartCoroutine(WaitForStoryText("对了，教程结束得先给你回个血……", ""));
        }
        stageCompleted = true;
    }
    IEnumerator Stage3() {
        arrowMax = 3; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("说实话啊，我觉得这个游戏有点无聊", ""));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("我看了看，这游戏往后的新内容就只剩一种换皮怪了！", ""));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("然后就是“在无限的怪物中生存下来吧！”的无尽模式……", ""));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("没有升级，没有装备，没有峰回路转的牛逼剧情", ""));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("并且最重要的是，“丢骰子”的主题呢？", ""));
        yield return StartCoroutine(WaitForStoryText("骰子、骰子……哪儿有骰子……？", ""));
        arrowMax = 0;
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitUntilClear(0, 8f, "这里一个骰子都没有！切题呢？作者到底咋想的？", ""));
        noDifficulty = true;
        yield return StartCoroutine(WaitForStoryText("要不然这样，我们随便拿个东西，把它弄成骰子算了", ""));
        yield return new WaitForSeconds(2f);
        InputM.DropDice(InputM.KeyType.Fire);
        SetStoryText("嘿，成了！", "");
        stageCompleted = true;
    }
    IEnumerator Stage4() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(10f, "看到右边掉下来的东西了么？把它捡起来摇一摇！", ""));
        stageCompleted = true;
    }
    IEnumerator Stage5() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(WaitForStoryText("噔噔噔……！", ""));
        yield return new WaitForSeconds(6f);
        yield return StartCoroutine(WaitForStoryText("你看，这就是天才制作的骰子", ""));
        yield return new WaitForSeconds(1.5f);
        Spawn(Spawner.EnemyType.Mover, 1);
        if (Player.hp < 3) {
            Player.hp = 3;
            yield return StartCoroutine(WaitForStoryText("让我帮你回个血，我们继续！", ""));
        } else {
            yield return StartCoroutine(WaitForStoryText("来，我们继续！", ""));
        }
        stageCompleted = true;
    }
    IEnumerator Stage6() {
        arrowMax = 1; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 3);
        yield return new WaitForSeconds(1.5f);
    }
    //IEnumerator test() {
    //    while (true) {
    //        Spawn(Spawner.EnemyType.Mover, 3);
    //        SetStoryText("先刷几个瞎晃悠的", "");
    //        yield return new WaitForSeconds(3f);

    //        Spawn(Spawner.EnemyType.Shooter);
    //        SetStoryText("炮塔！", "");
    //        yield return new WaitForSeconds(2f);

    //        Spawn(Spawner.EnemyType.Heavy, 2);
    //        SetStoryText("最后是两个贼硬的……", "");
    //        yield return StartCoroutine(WaitUntilClear(2));

    //        InputM.DropDice(Modules.RandomOne(InputM.keyTypes.ToList()));
    //        SetStoryText("来一个骰子吧……？", "");
    //        yield return StartCoroutine(WaitUntilPickDice());

    //        yield return StartCoroutine(WaitForStoryText("准备好，下一轮要来了！", ""));
    //    }
    //}
    int arrowMax = 0; float arrowTime = 4f;
    IEnumerator ArrowSpawner() {
        while (true) {
            yield return new WaitForSeconds(arrowTime);
            if (arrowMax > 0) Spawn(Spawner.EnemyType.Arrow, Modules.randomDefault.Next(0, arrowMax));
        }
    }

}
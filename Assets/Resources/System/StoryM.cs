using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Linq;

public class StoryM : MonoBehaviour {

    // 文本
    public Text textStory;
    public RectTransform shapeBack;
    /// <summary>
    /// 设置旁白，并返回预期时间。旁白字幕将在预期时间后消失。
    /// </summary>
    public float SetStoryText(string textCn, string textEn = "English") {
        AudioM.Play("sub");
        bool isCn = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)";
        textStory.text = isCn ? textCn : textEn;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textStory.rectTransform);
        // 动画
        DOTween.Kill("StoryBackground"); shapeBack.DOScaleX((textStory.rectTransform.rect.width + 40f) / 100f, 0.3f).SetEase(Ease.OutSine).SetId("StoryBackground");
        textStory.text = "";
        DOTween.Kill("StoryText"); textStory.DOText(isCn ? textCn : textEn, 0.3f).SetEase(Ease.OutSine).SetId("StoryText");
        // 消失
        float time = 1f + (isCn ? textCn.Length / 3f : textEn.Split(" ").Length / 4f);
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
        DOTween.Kill("StoryText");
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
        int id = 0;
        for (int i = 0; i < count; i++) {
            if (id >= spawners[type].Count) id = 0;
            spawners[type][id].Spawn();
            id++;
        }
    }

    // 流程控制
    IEnumerator WaitUntilClear(int remain, float waitTime = -1, string textCn = "", string textEn = "") {
        int targetStage = stage;
        while (GetEnemyCount() > remain && !respawning && targetStage == stage) {
            if (waitTime >= 0f && waitTime < 0.3f) SetStoryText(textCn, textEn);
            waitTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
    }
    int GetEnemyCount() => FindObjectsOfType<EnemyBase>().Length - FindObjectsOfType<EnemyArrow>().Length;
    IEnumerator WaitUntilPickDice(float waitTime = -1, string textCn = "", string textEn = "") {
        int targetStage = stage;
        while (GetDiceCount() > 0 && !respawning && targetStage == stage) {
            if (waitTime >= 0f && waitTime < 0.3f) SetStoryText(textCn, textEn);
            waitTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
    }
    int GetDiceCount() => FindObjectsOfType<DiceEntity>().Count(dice => !dice.pickedUp);

    // 故事
    public int stage = 0; private bool stageCompleted = true;
    public GameObject panDeath, panWin1, panWin2;
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
        AudioM.Play("player_die");
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
            _ = Modules.randomDefault.Next(0, 4) switch {
                0 => SetStoryText("完了，寄咯！", "Woops, you died!"),
                1 => SetStoryText("哦吼，下辈子加油吧", "Ouch, work harder next life!"),
                2 => SetStoryText("诶，怎么回事？", "Yo, what's happening?"),
                3 => SetStoryText("还好我没开极限模式……", "Luckily I didn't have hardcore mode on..."),
                _ => throw new System.NotImplementedException(),
            };
        } else {
            _ = Modules.randomDefault.Next(0, 3) switch {
                0 => SetStoryText("不是吧？不至于吧？", "What? How you die from that?"), //TODO: t 改了
                1 => SetStoryText("你是特么故意的吧！", "Excuse me? Are you doing this on purpose?"), //TODO: t 肯定语气
                2 => SetStoryText("嗯？", "Hey!"),
                _ => throw new System.NotImplementedException(),
            };
        }
        yield return new WaitForSeconds(0.5f);
        // 转为黑屏
        panDeath.GetComponent<Image>().DOFade(1, 0.3f); panDeath.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOFade(1, 1f);
        yield return new WaitForSeconds(0.3f);
        // 复原
        FindObjectsOfType<EnemyBase>().ToList().ForEach(e => Modules.DestroyGameObject(e.gameObject));
        Player.hp = 3;
        Player.instance.currentBullet = Player.instance.maxBullet;
        yield return new WaitForSeconds(3f);
        // 淡出黑屏，玩家重生
        respawning = false;
        panDeath.GetComponent<Image>().DOFade(0, 0.8f); panDeath.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOFade(0, 0.3f);
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
        yield return StartCoroutine(WaitForStoryText("哦，哦！你好，欢迎欢迎", "Oh, oh! Hello there!"));
        yield return StartCoroutine(WaitForStoryText("先来过个教程啥的吧", "Let's go through a quick tutrial before we start."));
        yield return StartCoroutine(WaitForStoryText("游戏的按键操作写在了右上角，应该是个人都看得懂", "All control buttons are shown on the top-right corner. I bet nobody can't understand that."));
        SetStoryText("你不如先拿它们俩试试按键吧！", "Now, let's try some buttons with these two cute guys!");
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitUntilClear(0, 10f, "快点把它干掉，然后才好继续……", "Come on! You have to defeat them to continue..."));
        stageCompleted = true;
    }
    IEnumerator Stage2() {
        arrowMax = 1; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("哦，还剩下最后一条教程……", "Oh, a final reminder before I leave...")); //TODO t
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("“弹药不够的时候记得右键换弹！”", "Don't forget to reload the weapon before you run out of ammo!")); //TODO t
        Spawn(Spawner.EnemyType.Shooter, 1);
        Spawn(Spawner.EnemyType.Mover, 1);
        SetStoryText("加油，新手教程结束了！", "There you go, that's end of the tutrial!");
        yield return StartCoroutine(WaitUntilClear(1, 8f, "仔细一看，这个角色的手上连枪都没有……", "Hmm... He does not even have a gun in his hand after a closer look."));
        if (Player.hp < 3) {
            Player.hp = 3;
            Spawn(Spawner.EnemyType.Mover, 1);
            yield return StartCoroutine(WaitForStoryText("对了，教程结束得先给你回个血……", "Whoops, I almost forgot to heal you after the tutrial."));
        }
        stageCompleted = true;
    }
    IEnumerator Stage3() {
        arrowMax = 3; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("说实话啊，我感觉这个游戏有点……敷衍", "To be honest, I think this game is kind of boring...")); //TODO: t 文案改了
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("我看了看，这游戏往后的新内容就只剩一种换皮怪了！", "Let's see... We only have one more skin to replace our monsters!"));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("除此之外就只剩无尽模式了……但无尽模式谁玩啊？", "And then, all we have is endless mode of \"Survive with infinite monsters!\"...")); //TODO: t 文案改了
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("没有升级，没有装备，没有峰回路转的牛逼剧情", "No upgrade, no equipments, no twist to the plot or any boss to fight."));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("并且最重要的是，说好的“丢骰子”呢？", "And most importantly, where's our \"Roll of the Dice\" theme?")); //TODO t
        yield return StartCoroutine(WaitForStoryText("骰子、骰子……让我找找哪儿有骰子……？", "Dice... Dice... Where is the dice...?")); //TODO: t
        SetStoryText("这个游戏里一个骰子都没有！", "Not a single dice here!"); //TODO: t
        arrowMax = 0;
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 2 : 1);
        yield return StartCoroutine(WaitUntilClear(0, 6f, "切题呢？作者到底咋想的？", "What's the point? What's wrong with this author?"));
        noDifficulty = true;
        yield return StartCoroutine(WaitForStoryText("要不然这样，我们随便拿个东西，把它弄成骰子算了", "How about we just grab something and make it a dice?"));
        yield return new WaitForSeconds(2f);
        InputM.DropDice(InputM.KeyType.Fire);
        SetStoryText("嘿，成了！", "Let's go!");
        stageCompleted = true;
    }
    IEnumerator Stage4() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(8f, "看到右边掉下来的东西了么？把它捡起来摇一摇！", "You see the falling stuff on the right? Pick it up and try shaking it!"));
        stageCompleted = true;
    }
    IEnumerator Stage5() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WaitForStoryText("噔噔噔……！", "Pow!")); //TODO: t 这是在说骰子的音效
        yield return new WaitForSeconds(7f);
        yield return StartCoroutine(WaitForStoryText("你看，这就是天才制作的骰子", "See, what a master piece!")); //TODO: t 是骰子！
        yield return new WaitForSeconds(1.5f);
        Spawn(Spawner.EnemyType.Mover, 1);
        if (Player.hp < 3) {
            Player.hp = 3;
            yield return StartCoroutine(WaitForStoryText("让我帮你回个血，我们继续！", "Let me heal you first and then keep going!"));
        } else {
            yield return StartCoroutine(WaitForStoryText("来，我们继续！", "Let's keep going!"));
        }
        stageCompleted = true;
    }
    IEnumerator Stage6() {
        arrowMax = 1; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 3);
        yield return StartCoroutine(WaitUntilClear(1, 6f, "骰子有六面，按键有六个，简直完美！", "Six sides to six buttons, perfect!"));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("感觉如何？", "How do you feel?"));
        arrowMax = deathInStage <= 1 ? 3 : 1;
        yield return StartCoroutine(WaitForStoryText("是不是还想马上再来一颗？", "Want to have another dice now?"));
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("但如果给你太多骰子，那岂不是显得它很……普通", "But if I give you too many dices, it will look... ordinary."));
        arrowMax = 1;
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("不如让我们先等等，我先给你看看之前说的新敌人", "Why don't we wait while I show you the new enemy we were talking about...?"));
        SetStoryText("额……就是换皮的那个", "Ugh... That monster with new skin!");
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitUntilClear(1, 6f, "别抱太大期望就是了……", "Just don't hope too much on that..."));
        stageCompleted = true;
    }
    IEnumerator Stage7() {
        arrowMax = 0; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("让我找找它在哪……", "Hold on... I can't find where it is."));
        yield return new WaitForSeconds(1.5f);
        SetStoryText("卧槽！小心！", ""); // TODO: t 这里说的是要掉下来一堆圆锯片了
        for (int i = 0; i < Mathf.Max(6, 15 - deathInStage * 2); i++) {
            Spawn(Spawner.EnemyType.Mover, 1);
            yield return new WaitForSeconds(0.4f);
        }
        yield return StartCoroutine(WaitForStoryText("我把圆锯片的仓库弄漏了……", "I mix up the wrong repository for circular saw blades..."));
        yield return StartCoroutine(WaitForStoryText("明明打算之后用来做点装饰的，唉", "Awww... I was gonna use it for decoration later."));
        yield return StartCoroutine(WaitUntilClear(6, 6f, "出了这么大问题，玩家给游戏打差评怎么办？", ""));
        if (Player.hp < 3) {
            Player.hp = 3;
            yield return StartCoroutine(WaitForStoryText("我帮你回个血吧……唉，惹麻烦了……", ""));
        } else {
            yield return StartCoroutine(WaitForStoryText("唉，惹麻烦了……", ""));
        }
        yield return StartCoroutine(WaitUntilClear(2, 7f, "我的锯片……", ""));
        yield return StartCoroutine(WaitForStoryText("要不然……我再给你一颗特制按键骰子™作为补偿？", ""));
        yield return new WaitForSeconds(1f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Left, InputM.KeyType.Right, InputM.KeyType.Crouch }.RandomOne());
        SetStoryText("你的补偿来了！", "");
        stageCompleted = true;
    }
    IEnumerator Stage8() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(6f, "难道说你不喜欢特制按键骰子™吗？别吧", ""));
        stageCompleted = true;
    }
    IEnumerator Stage9() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(7f);
        stageCompleted = true;
    }
    IEnumerator Stage10() {
        arrowMax = deathInStage <= 2 ? 2 : 0; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("唔，看起来特制按键骰子™的威力还挺大……", ""));
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 4 : 3);
        yield return new WaitForSeconds(1.5f);
        SetStoryText("呼，圆锯片看起来也还剩了不少", "");
        yield return StartCoroutine(WaitUntilClear(1, 10f, "哇哦……", ""));
        Spawn(Spawner.EnemyType.Shooter, deathInStage <= 1 ? 3 : 2);
        yield return StartCoroutine(WaitForStoryText("看起来我低估了骰子™的……", ""));
        SetStoryText("刺激程度", "");
        yield return StartCoroutine(WaitUntilClear(2, 10f, "这不比原来好玩多了嘛！", ""));
        stageCompleted = true;
    }
    IEnumerator Stage11() {
        arrowMax = 2; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("既然骰子™能为游戏的玩法带来颠覆性的体验", ""));
        SetStoryText("那我们为啥不再立即多来几颗呢？这不是摆明了越多骰子就越好玩么？", "");
        Spawn(Spawner.EnemyType.Heavy, deathInStage <= 0 ? 2 : 1);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WaitForStoryText("哈？", ""));
        arrowMax = 3;
        Spawn(Spawner.EnemyType.Heavy, 1);
        yield return StartCoroutine(WaitForStoryText("螺母怎么在这个时候出来了？", ""));
        Spawn(Spawner.EnemyType.Shooter, deathInStage <= 1 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("额……它就是我之前说的那个换皮怪", ""));
        Spawn(Spawner.EnemyType.Heavy, 2);
        yield return StartCoroutine(WaitForStoryText("但它就是最后一种敌人了！", ""));
        SetStoryText("这下就没有任何新东西了！", "");
        yield return StartCoroutine(WaitUntilClear(1, 10f, "今天真是诸事不宜……", ""));
        stageCompleted = true;
    }
    IEnumerator Stage12() {
        arrowMax = 0; noDifficulty = true;
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("没有新东西就没有悬念，那玩家就会直接 Alt+F4 走人了", ""));
        yield return StartCoroutine(WaitForStoryText("不行……", ""));
        SetStoryText("我们需要更多的特制按键骰子™！", "");
        yield return new WaitForSeconds(1.5f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Jump, InputM.KeyType.Reload }.RandomOne());
        stageCompleted = true;
    }
    IEnumerator Stage13() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(10f, "拜托，我就只剩下骰子™了，你不捡还能干什么？", ""));
        stageCompleted = true;
    }
    IEnumerator Stage14() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(8f);
        stageCompleted = true;
    }
    IEnumerator Stage15() {
        arrowMax = 2; noDifficulty = false;
        Spawn(Spawner.EnemyType.Shooter, deathInStage <= 1 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("哇……三个骰子的威力……", ""));
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 3 : 2);
        yield return new WaitForSeconds(5f);
        Spawn(Spawner.EnemyType.Mover, 1);
        if (Player.hp < 3) {
            Player.hp = 3; arrowMax = 1;
            yield return StartCoroutine(WaitForStoryText("咳咳，总之，先来回个血吧", ""));
        } else {
            yield return StartCoroutine(WaitForStoryText("比我想像中的要猛啊……", ""));
        }
        yield return new WaitForSeconds(2f);
        SetStoryText("好吧，我承认我可能做得有点过火了……", "");
        yield return StartCoroutine(WaitUntilClear(2, 10f, "骰子™让我太兴奋了", ""));
        Spawn(Spawner.EnemyType.Heavy, 2);
        yield return StartCoroutine(WaitForStoryText("我保证！我之后不会再放那么多骰子™了！", ""));
        yield return new WaitForSeconds(2f);
        stageCompleted = true;
    }
    IEnumerator Stage16() {
        arrowMax = Mathf.Max(4, 9 - deathInStage); arrowTime = 2f; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("诶？", ""));
        yield return StartCoroutine(WaitForStoryText("我们似乎到 BOSS 关卡了", ""));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("似乎……就是一堆垫圈满天飞？倒也符合这游戏的尿性", ""));
        yield return new WaitForSeconds(3f);
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("打完 BOSS 关后，就进入无尽模式了", ""));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("嗯……就是敌人会出现得越来越快啊，有个计时器啊，巴拉巴拉", ""));
        yield return StartCoroutine(WaitUntilClear(0, 10f, "说起来这游戏又没排行榜，那计时器有啥意义？", ""));
        stageCompleted = true;
    }
    IEnumerator Stage17() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitForStoryText("反正，无尽模式差不多就是这么回事", ""));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(WaitForStoryText("嗯？垫圈停了？结束了？", ""));
        yield return new WaitForSeconds(1f);
        SetStoryText("那么，在开始无尽模式之前……！", "");
        yield return new WaitForSeconds(1f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Left, InputM.KeyType.Right }.RandomOne(), DiceEntity.DiceType.Bad);
        stageCompleted = true;
    }
    IEnumerator Stage18() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(3f, "无尽模式里我肯定不会再丢骰子™了，真的！", ""));
        stageCompleted = true;
    }
    IEnumerator Stage19() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(11f);
        yield return StartCoroutine(WaitForStoryText("呃……", ""));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("这可……不太妙……", ""));
        yield return StartCoroutine(WaitForStoryText("后面还有无尽模式啊！无尽模式！", ""));
        yield return StartCoroutine(WaitForStoryText("这可咋整啊？", ""));
        yield return new WaitForSeconds(2f);
        SetStoryText("不行！我再想想办法，如果再重新丢一次骰子™，指不定就好了呢？丢一次不行就两次，对吧？", "");
        yield return new WaitForSeconds(1.5f);
        // 结束动画
        respawning = true;
        Player.instance.transform.localScale = Vector3.zero;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.enabled = false;
        panWin1.SetActive(true);
        yield return new WaitForSeconds(1f);
        SetStoryText("诶？喂？", "");
        yield return new WaitForSeconds(8f);
        panWin1.SetActive(false); panWin2.SetActive(true);
        Text credit = panWin2.GetComponentInChildren<Text>();
        string creditText = credit.text;
        credit.text = "";
        credit.DOText(creditText, 25f).SetEase(Ease.Linear);
    }


    int arrowMax = 0; float arrowTime = 4f;
    IEnumerator ArrowSpawner() {
        while (true) {
            yield return new WaitForSeconds(arrowTime);
            if (arrowMax > 0) Spawn(Spawner.EnemyType.Arrow, Modules.randomDefault.Next(0, arrowMax));
        }
    }

}
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
        float time = 1f + (isCn ? textCn.Length / 3f : textEn.Split(" ").Length / 3f);
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
        if (stage == 16 && deathInStage % 3 == 1) {
            SetStoryText("嘿，你可以蹲下来躲避飞在天上的螺圈！", "Hey, you can dodge these flying coils by crouching down!");
        } else if (!noDifficulty) {
            _ = Modules.randomDefault.Next(0, 6) switch {
                0 => SetStoryText("完了，寄咯！", "Woops, you died!"),
                1 => SetStoryText("哦吼，下辈子加油吧", "Ouch, work harder next life!"),
                2 => SetStoryText("诶，怎么回事？", "Yo, what's happening?"),
                3 => SetStoryText("还好我没开极限模式……", "Luckily I didn't have hardcore mode on..."),
                4 => SetStoryText("那叫啥来着……保持你的决心！", "What's that called... KEEP YOUR DETERMINATION!"),
                5 => SetStoryText("嗨呀，再多试试吧，能过的啦", "Hey, try again, you can do it!"),
                _ => throw new System.NotImplementedException(),
            };
        } else {
            _ = Modules.randomDefault.Next(0, 3) switch {
                0 => SetStoryText("不是吧？不至于吧？", "What? How?"),
                1 => SetStoryText("你是特么故意的吧！", "Are you doing this on purpose?"),
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
        yield return StartCoroutine(WaitForStoryText("先来过个教程啥的吧", "Let's go through a quick tutorial before we start."));
        yield return StartCoroutine(WaitForStoryText("游戏的按键操作写在了右上角，应该是个人都看得懂", "All control buttons are shown on the top-right corner. I bet nobody can't understand that."));
        SetStoryText("你不如先拿它们俩试试按键吧！", "Now, let's try some buttons with these two cute guys!");
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitUntilClear(0, 15f, "快点把它干掉，然后才好继续……", "Come on! You have to defeat them to continue..."));
        stageCompleted = true;
    }
    IEnumerator Stage2() {
        arrowMax = 1; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("哦，还剩下最后一条教程……", "Oh, a final reminder before I leave..."));
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("“弹药不够的时候记得右键换弹！”", "Don't forget to reload the weapon using right click before you run out of ammo!"));
        Spawn(Spawner.EnemyType.Shooter, 1);
        Spawn(Spawner.EnemyType.Mover, 1);
        SetStoryText("加油，新手教程结束了！", "There you go, that's end of the tutorial.");
        yield return StartCoroutine(WaitUntilClear(1, 8f, "仔细一看，这个角色的手上连枪都没有……", "Hmm... He does not even have a gun in his hand after a closer look."));
        if (Player.hp < 3) {
            Player.hp = 3;
            Spawn(Spawner.EnemyType.Mover, 1);
            yield return StartCoroutine(WaitForStoryText("对了，教程结束得先给你回个血……", "Whoops, I almost forgot to heal you after the tutorial."));
        }
        stageCompleted = true;
    }
    IEnumerator Stage3() {
        arrowMax = 3; noDifficulty = false;
        Spawn(Spawner.EnemyType.Mover, 3);
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(WaitForStoryText("说实话啊，我感觉这个游戏有点……敷衍", "To be honest, I think this game is kind of perfunctory..."));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("我看了看，这游戏往后的新内容就只剩一种换皮怪了！", "Let's see... We only have one more new enemy with ONLY new skin!"));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("除此之外就只剩无尽模式了……但无尽模式谁玩啊？", "And then, all we have is endless mode... But who plays endless mode?"));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("没有升级，没有装备，没有峰回路转的牛逼剧情", "No upgrade, no equipments, no twist to the plot..."));
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitForStoryText("并且最重要的是，说好的“丢骰子”呢？", "And most importantly, where's \"Roll of the Dice\"?"));
        yield return StartCoroutine(WaitForStoryText("骰子、骰子……让我找找哪儿有骰子……？", "Dice... Dice... Where is the dice...?"));
        SetStoryText("这个游戏里一个骰子都没有！", "Not a single dice in this game!");
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
        yield return StartCoroutine(WaitForStoryText("噔噔噔……！", "Ding ding ding!"));
        yield return new WaitForSeconds(7f);
        yield return StartCoroutine(WaitForStoryText("你看，这就是天才制作的骰子", "See, what a genius-made dice!"));
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
        yield return StartCoroutine(WaitUntilClear(1, 8f, "骰子有六面，按键有六个，简直完美！", "Six sides to six buttons, perfect!"));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("感觉如何？", "How do you feel?"));
        arrowMax = deathInStage <= 1 ? 3 : 1;
        yield return StartCoroutine(WaitForStoryText("是不是还想马上再来一颗？", "Want to have another dice now?"));
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return StartCoroutine(WaitForStoryText("但如果给你太多骰子，那岂不是显得它很……普通", "But if I give you too many dice, it will look... ordinary."));
        arrowMax = 3;
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("不如让我们先等等，我先给你看看之前说的新敌人", "Why don't we wait while I show you the new enemy we were talking about...?"));
        SetStoryText("额……就是换皮的那个", "Ugh... That enemy with ONLY new skin.");
        Spawn(Spawner.EnemyType.Mover, 1);
        yield return StartCoroutine(WaitUntilClear(1, 6f, "别抱太大期望就是了……", "So just don't hope too much on that..."));
        stageCompleted = true;
    }
    IEnumerator Stage7() {
        arrowMax = 0; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("让我找找那个换皮怪在哪……", "Hold on... I can't find where that enemy is."));
        yield return new WaitForSeconds(1.5f);
        SetStoryText("卧槽！小心！", "Geez! Watch out!");
        for (int i = 0; i < Mathf.Max(6, 15 - deathInStage * 2); i++) {
            Spawn(Spawner.EnemyType.Mover, 1);
            yield return new WaitForSeconds(0.4f);
        }
        yield return StartCoroutine(WaitForStoryText("我把圆锯片的仓库弄漏了……", "I mix up the wrong repository for saw blades..."));
        yield return StartCoroutine(WaitForStoryText("明明打算之后给宫殿做点装饰的，唉", "Aww... I was gonna use it for decoration in the palace later."));
        yield return StartCoroutine(WaitUntilClear(6, 6f, "出了这么大问题，玩家给游戏打差评怎么办？", "What if players give bad reviews of this game with such a big issue?"));
        if (Player.hp < 3) {
            Player.hp = 3;
            yield return StartCoroutine(WaitForStoryText("我帮你回个血吧……唉，惹麻烦了……", "Aww, that's a huge trouble... Let me heal you first."));
        } else {
            yield return StartCoroutine(WaitForStoryText("唉，惹麻烦了……", "Aww, that's a huge trouble..."));
        }
        yield return StartCoroutine(WaitUntilClear(3, 6f, "我的锯片……", "My saw blades..."));
        yield return StartCoroutine(WaitForStoryText("要不然……我再给你一颗特制按键骰子™作为补偿？", "How about... I give you another specially made Button Dice™ for compensation?"));
        yield return new WaitForSeconds(1f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Left, InputM.KeyType.Right, InputM.KeyType.Crouch }.RandomOne());
        SetStoryText("你的补偿来了！", "Here is your compensation!");
        stageCompleted = true;
    }
    IEnumerator Stage8() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(10f, "难道说你不喜欢特制按键骰子™吗？别吧", "Come on! Don't tell me you don't like my Button Dice™!"));
        stageCompleted = true;
    }
    IEnumerator Stage9() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(7f);
        stageCompleted = true;
    }
    IEnumerator Stage10() {
        arrowMax = deathInStage <= 2 ? 2 : 0; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("唔，看起来特制按键骰子™的威力还挺大……", "Hmm, looks like the Button Dice™ is pretty powerful..."));
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 4 : 3);
        yield return new WaitForSeconds(1.5f);
        SetStoryText("呼，圆锯片看起来也还剩了不少", "Whew, seems to have a lot saw blades left.");
        yield return StartCoroutine(WaitUntilClear(1, 12f, "哇哦……", "Wow!"));
        Spawn(Spawner.EnemyType.Shooter, deathInStage <= 1 ? 3 : 2);
        yield return StartCoroutine(WaitForStoryText("看起来我低估了骰子™的……", "It seems that I have under underestimate the Button Dice™'s..."));
        SetStoryText("刺激程度", "Intensity.");
        yield return StartCoroutine(WaitUntilClear(2, 12f, "这不比原来好玩多了嘛！", "Isn't this more fun than before?"));
        stageCompleted = true;
    }
    IEnumerator Stage11() {
        arrowMax = 2; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("既然骰子™能为游戏的玩法带来颠覆性的体验", "Since Dice™ can bring such a revolutionary experience to the gameplay..."));
        SetStoryText("那我们为啥不再立即多来几颗呢？这不是摆明了越多骰子™就越好玩么？", "So why don't we just get a few more Dice™ now? Wouldn't more Dice™ be more fun?");
        Spawn(Spawner.EnemyType.Heavy, deathInStage <= 0 ? 2 : 1);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WaitForStoryText("哈？", "Hah?"));
        arrowMax = 4;
        Spawn(Spawner.EnemyType.Heavy, 1);
        yield return StartCoroutine(WaitForStoryText("螺母怎么在这个时候出来了？", "How did the nut come out at this time?"));
        Spawn(Spawner.EnemyType.Shooter, deathInStage <= 1 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("额……它就是我之前说的那个换皮怪", "Ugh... That's the monster what we talking about."));
        Spawn(Spawner.EnemyType.Heavy, deathInStage <= 1 ? 2 : 1);
        yield return StartCoroutine(WaitForStoryText("但它就是最后一种敌人了！", "But it will be our final type of enemies!"));
        SetStoryText("没有新敌人、没有新机制……", "No more new enemies, no more new mechanics...");
        yield return StartCoroutine(WaitUntilClear(1, 10f, "今天真是诸事不宜……", "What an awful day..."));
        stageCompleted = true;
    }
    IEnumerator Stage12() {
        arrowMax = 0; noDifficulty = true;
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("如果没有悬念，那玩家就会直接 Alt+F4 走人了", "If there is no suspense, the player will just press Alt+F4 to leave"));
        yield return StartCoroutine(WaitForStoryText("不行……", "No..."));
        SetStoryText("我们需要更多的特制按键骰子™！", "We need more special Button Dice™!");
        yield return new WaitForSeconds(1.5f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Jump, InputM.KeyType.Reload }.RandomOne());
        stageCompleted = true;
    }
    IEnumerator Stage13() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(10f, "拜托，我就只剩下骰子™了，你不捡还能干什么？", "Come on, all I have is Dice™. What else can you do if you don't pick them up?"));
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
        yield return StartCoroutine(WaitForStoryText("哇……三个骰子的威力……", "Wow, the power of three dice..."));
        Spawn(Spawner.EnemyType.Mover, deathInStage <= 0 ? 3 : 2);
        yield return new WaitForSeconds(5f);
        Spawn(Spawner.EnemyType.Mover, 1);
        if (Player.hp < 3) {
            Player.hp = 3; arrowMax = 1;
            yield return StartCoroutine(WaitForStoryText("咳咳，总之，先帮你回个血吧", "Ahem... Anyway, let me heal you first."));
        } else {
            yield return StartCoroutine(WaitForStoryText("比我想像中的要猛啊……", "That was a lot harder than I thought..."));
        }
        yield return new WaitForSeconds(2f);
        SetStoryText("好吧，我承认我可能做得有点过火了……", "Okay, I admit I may have gone a bit too far...");
        yield return StartCoroutine(WaitUntilClear(2, 10f, "骰子™可能让我太兴奋了……", "I'm probably too excited about the Dice™..."));
        Spawn(Spawner.EnemyType.Heavy, 1);
        yield return StartCoroutine(WaitForStoryText("我保证！我之后不会再放那么多骰子™了！", "I promise! I will not put that many Dice™ again!"));
        yield return new WaitForSeconds(3f);
        stageCompleted = true;
    }
    IEnumerator Stage16() {
        arrowMax = Mathf.Max(3, 5 - deathInStage); arrowTime = 1.5f + deathInStage * 0.1f; noDifficulty = false;
        yield return StartCoroutine(WaitForStoryText("诶？", "Eh?"));
        yield return StartCoroutine(WaitForStoryText("我们似乎到 BOSS 关卡了", "I think we're at the boss level."));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("似乎……就是一堆螺圈满天飞？倒也符合这游戏的尿性", "Well... just a bunch of coils flying over the sky? At least it fits the nature of this game."));
        Spawn(Spawner.EnemyType.Mover, 2);
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("打完 BOSS 关后，就进入无尽模式了", "After finishing the boss level, you will enter the endless mode."));
        Spawn(Spawner.EnemyType.Shooter, 1);
        yield return StartCoroutine(WaitForStoryText("嗯……就是敌人会出现得越来越快啊，有个计时器啊，巴拉巴拉", "Hmm... it's just enemies will appear faster and faster, and there will be a timer, blablabla..."));
        yield return StartCoroutine(WaitUntilClear(0, 10f, "说起来这游戏又没排行榜，那计时器有啥意义？", "Speaking of the timer, what's the point if there's not a leaderboard?"));
        stageCompleted = true;
    }
    IEnumerator Stage17() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitForStoryText("反正，无尽模式差不多就是这么回事", "Anyway, that's pretty much what the endless mode is..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("嗯？螺圈停了？结束了？", "Huh? Did the coil stop? Is this the end?"));
        yield return new WaitForSeconds(1f);
        SetStoryText("那么，在开始无尽模式之前……！", "So, before we start the endless mode...!");
        yield return new WaitForSeconds(1f);
        InputM.DropDice(new List<InputM.KeyType>() { InputM.KeyType.Left, InputM.KeyType.Right }.RandomOne(), DiceEntity.DiceType.Bad);
        stageCompleted = true;
    }
    IEnumerator Stage18() {
        arrowMax = 0; noDifficulty = true;
        yield return StartCoroutine(WaitUntilPickDice(3f, "无尽模式里我肯定不会再丢骰子™了，真的！", "I'm definitely not going to roll the Dice™ again in the endless mode, really!"));
        stageCompleted = true;
    }
    IEnumerator Stage19() {
        arrowMax = 0; noDifficulty = true;
        yield return new WaitForSeconds(11f);
        yield return StartCoroutine(WaitForStoryText("呃……", "Ueh..."));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(WaitForStoryText("这可……不太妙……", "That's... not good..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForStoryText("后面还有无尽模式啊！无尽模式！这样可怎么玩？", "There is an endless mode after this! Endless mode! How can this be played?"));
        yield return StartCoroutine(WaitForStoryText("这可咋整啊？", "What should I do?"));
        yield return new WaitForSeconds(2f);
        SetStoryText("不行！我再想想办法，如果再重新丢一次骰子™，指不定就好了呢？丢一次不行就两次，对吧？", "No way! Let me think about it... What if I can fix it by rolling the Dice™ again?");
        yield return new WaitForSeconds(1.5f);
        // 结束动画
        respawning = true;
        Player.instance.transform.localScale = Vector3.zero;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.enabled = false;
        panWin1.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WaitForStoryText("诶？喂？", "Aye? Hello?"));
        SetStoryText("无尽模式呢？", "Where is the endless mode?");
        yield return new WaitForSeconds(6f);
        panWin1.SetActive(false); panWin2.SetActive(true);
        Text credit = panWin2.GetComponentInChildren<Text>();
        credit.text = "";
        bool isCn = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)";
        const string cn = "<b>作者：</b>Hex Dragon\n　　　龙腾猫跃、00ll00、HerobrineXia\n\n制作于  GMTK  GameJam  2022                                                      \n\n<color=#404040>哦，对了，我们似乎忘了加关闭游戏按钮了？           \n那……你要不然试试按  Alt + F4……？                                                      </color>\n\n感谢你的游玩！";
        const string en = "<b>Author：</b>Hex Dragon\n　　      　LTCat、00ll00、HerobrineXia\n\nMade  for  GMTK  GameJam  2022                                                      \n\n<color=#404040>Oh yes, it seems that we forget to add the close button?           \nWell... You might want try to press Alt + F4...？                                                      </color>\n\nThanks for playing！";
        credit.DOText(isCn ? cn : en, 25f).SetEase(Ease.Linear);
    }


    int arrowMax = 0; float arrowTime = 4f;
    IEnumerator ArrowSpawner() {
        while (true) {
            yield return new WaitForSeconds(arrowTime);
            if (arrowMax > 0) Spawn(Spawner.EnemyType.Arrow, Modules.randomDefault.Next(0, arrowMax));
        }
    }

}
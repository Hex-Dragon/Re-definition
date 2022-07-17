using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputM : MonoBehaviour {
    public GameObject dicePrefab;

    public static string keyLeft = "a", keyRight = "d", keyJump = "w", keyCrouch = "s", keyFire = "l", keyReload = "r";
    public enum KeyType { Left, Right, Jump, Crouch, Fire, Reload }

    public static void SetKey(KeyType key, string newKey) {
        _ = key switch {
            KeyType.Left => keyLeft = newKey,
            KeyType.Right => keyRight = newKey,
            KeyType.Jump => keyJump = newKey,
            KeyType.Crouch => keyCrouch = newKey,
            KeyType.Fire => keyFire = newKey,
            _ => keyReload = newKey,
        };
        foreach (GameObject dice in instance.dicesUI) dice.GetComponent<DiceUI>().UpdateLetter();
    }
    public static bool GetKeyUI(KeyType key) => key switch {
        KeyType.Left => keyLeft == "l" ? Input.GetMouseButton(0) : (keyLeft == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyLeft)),
        KeyType.Right => keyRight == "l" ? Input.GetMouseButton(0) : (keyRight == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyRight)),
        KeyType.Jump => keyJump == "l" ? Input.GetMouseButton(0) : (keyJump == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyJump)),
        KeyType.Crouch => keyCrouch == "l" ? Input.GetMouseButton(0) : (keyCrouch == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyCrouch)),
        KeyType.Fire => keyFire == "l" ? Input.GetMouseButton(0) : (keyFire == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyFire)),
        _ => keyReload == "l" ? Input.GetMouseButton(0) : (keyReload == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyReload)),
    };
    public static bool GetKeyEvent(KeyType key) => key switch {
        KeyType.Left => keyLeft == "l" ? Input.GetMouseButton(0) : (keyLeft == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyLeft)),
        KeyType.Right => keyRight == "l" ? Input.GetMouseButton(0) : (keyRight == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyRight)),
        KeyType.Jump => keyJump == "l" ? Input.GetMouseButtonDown(0) : (keyJump == "r" ? Input.GetMouseButtonDown(1) : Input.GetKeyDown(keyJump)),
        KeyType.Crouch => keyCrouch == "l" ? Input.GetMouseButton(0) : (keyCrouch == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyCrouch)),
        KeyType.Fire => keyFire == "l" ? Input.GetMouseButton(0) : (keyFire == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyFire)),
        _ => keyReload == "l" ? Input.GetMouseButtonDown(0) : (keyReload == "r" ? Input.GetMouseButtonDown(1) : Input.GetKeyDown(keyReload)),
    };
    public static string GetKeyRaw(KeyType key) => key switch {
        KeyType.Left => keyLeft,
        KeyType.Right => keyRight,
        KeyType.Jump => keyJump,
        KeyType.Crouch => keyCrouch,
        KeyType.Fire => keyFire,
        _ => keyReload,
    };
    public static string GetKeyRawDefault(KeyType key) => key switch {
        KeyType.Left => "a",
        KeyType.Right => "d",
        KeyType.Jump => "w",
        KeyType.Crouch => "s",
        KeyType.Fire => "l",
        _ => "r",
    };

    public static InputM instance;
    private void Awake() {
        instance = this;
        keyLeft = "a"; keyRight = "d"; keyJump = "w"; keyCrouch = "s"; keyFire = "l"; keyReload = "r";
    }

    public Sprite[] spritesText, spritesType;
    public GameObject[] dicesUI;
    public static Sprite GetKeyTextSprite(string key) => key switch {
        "w" => instance.spritesText[0],
        "a" => instance.spritesText[1],
        "s" => instance.spritesText[2],
        "d" => instance.spritesText[3],
        "l" => instance.spritesText[4],
        _ => instance.spritesText[5],
    };
    public static Sprite GetKeyTypeSprite(KeyType key) => key switch {
        KeyType.Jump => instance.spritesType[0],
        KeyType.Left => instance.spritesType[1],
        KeyType.Crouch => instance.spritesType[2],
        KeyType.Right => instance.spritesType[3],
        KeyType.Fire => instance.spritesType[4],
        _ => instance.spritesType[5],
    };
    public static GameObject GetDiceUI(KeyType key) => key switch {
        KeyType.Jump => instance.dicesUI[0],
        KeyType.Left => instance.dicesUI[1],
        KeyType.Crouch => instance.dicesUI[2],
        KeyType.Right => instance.dicesUI[3],
        KeyType.Fire => instance.dicesUI[4],
        _ => instance.dicesUI[5],
    };

    public readonly static KeyType[] keyTypes = new[] { KeyType.Left, KeyType.Right, KeyType.Jump, KeyType.Crouch, KeyType.Fire, KeyType.Reload };
    public static string GetPossibleResults(KeyType key, bool doRestore) {
        List<string> accepted = "wasdlr".ToList().Select(c => c.ToString()).ToList();
        List<string> refused = new();
        // 删除冲突键位
        //  - 不能同时跳跃、蹲下
        if (key == KeyType.Jump) refused.Add(GetKeyRaw(KeyType.Crouch));
        if (key == KeyType.Crouch) refused.Add(GetKeyRaw(KeyType.Jump));
        //  - 不能同时向左、向右
        if (key == KeyType.Left) refused.Add(GetKeyRaw(KeyType.Right));
        if (key == KeyType.Right) refused.Add(GetKeyRaw(KeyType.Left));
        //  - 不能同时开火、装弹
        if (key == KeyType.Fire) refused.Add(GetKeyRaw(KeyType.Reload));
        if (key == KeyType.Reload) refused.Add(GetKeyRaw(KeyType.Fire));
        // 禁止选择已经有两个绑定项的字母、尽量选择没有绑定项的字母
        foreach (string letter in "wasdlr".ToCharArray().Select(c => c.ToString())) {
            if (keyTypes.Count(keyType => GetKeyRaw(keyType) == letter) >= 2) refused.Add(letter);
            if (keyTypes.Count(keyType => GetKeyRaw(keyType) == letter) == 0) { accepted.Add(letter); accepted.Add(letter); }
        }
        // 禁止与当前项相同
        refused.Add(GetKeyRaw(key));
        // 是否可以还原回原始键位
        if (!doRestore) refused.Add(GetKeyRawDefault(key));
        if (doRestore) for (int i = 0; i < 10; i++) accepted.Add(GetKeyRawDefault(key));
        // 输出
        string resultStr = "";
        accepted.ForEach(resultChar => {
            foreach (string refusedChar in refused) if (resultChar == refusedChar) return;
            resultStr += resultChar;
        });
        return resultStr;
    }

    public static void DropDice(KeyType key) {
        GetDiceUI(key).GetComponent<DiceUI>().DropDice();
    }

}
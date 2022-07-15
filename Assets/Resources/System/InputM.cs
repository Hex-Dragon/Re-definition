using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class InputM : MonoBehaviour {

    public void Update() {
        // 检测键盘按下
        foreach (KeyCode keyCode in keyboardEvent.Keys) {
            if (Input.GetKeyUp(keyCode)) TriggerKeyboardEvent(keyCode);
        }
        // 检测鼠标按下
        int? mouseButton = null;
        if (Input.GetMouseButtonUp(0)) mouseButton = 0;
        if (Input.GetMouseButtonUp(1)) mouseButton = 1;
        if (mouseButton == null) return;
    }

    // 键盘按键绑定
    public delegate void KeyboardEventDelegate();
    private static Dictionary<KeyCode, KeyboardEventDelegate> keyboardEvent;
    public void Awake() {
        keyboardEvent = new();
    }
    public static void AddKeyboardEvent(KeyCode key, KeyboardEventDelegate onKeyClicked) {
        Debug.Log("添加按键绑定：" + key.ToString());
        keyboardEvent[key] = onKeyClicked;
    }
    public void TriggerKeyboardEvent(KeyCode key) {
        Debug.Log("触发按键：" + key.ToString());
        keyboardEvent[key].Invoke();
    }

}

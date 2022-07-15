using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class InputM : MonoBehaviour {

    public void Update() {
        // �����̰���
        foreach (KeyCode keyCode in keyboardEvent.Keys) {
            if (Input.GetKeyUp(keyCode)) TriggerKeyboardEvent(keyCode);
        }
        // �����갴��
        int? mouseButton = null;
        if (Input.GetMouseButtonUp(0)) mouseButton = 0;
        if (Input.GetMouseButtonUp(1)) mouseButton = 1;
        if (mouseButton == null) return;
    }

    // ���̰�����
    public delegate void KeyboardEventDelegate();
    private static Dictionary<KeyCode, KeyboardEventDelegate> keyboardEvent;
    public void Awake() {
        keyboardEvent = new();
    }
    public static void AddKeyboardEvent(KeyCode key, KeyboardEventDelegate onKeyClicked) {
        Debug.Log("��Ӱ����󶨣�" + key.ToString());
        keyboardEvent[key] = onKeyClicked;
    }
    public void TriggerKeyboardEvent(KeyCode key) {
        Debug.Log("����������" + key.ToString());
        keyboardEvent[key].Invoke();
    }

}

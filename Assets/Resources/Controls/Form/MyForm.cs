using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyForm : MonoBehaviour {
    public KeyCode key = KeyCode.None;
    public bool defaultShow = false;
    private Vector2 originalPosition;
    public void Awake() {
        // 绑定显示按键（切换显示状态）
        if (key != KeyCode.None) {
            InputM.AddKeyboardEvent(key, () => SwapShowing());
        }
        // 保存初始位置
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    // 设置或更改窗口的显示状态
    public void SwapShowing() {
        FormM.currentForm = (FormM.currentForm == this ? null : this);
    }
    public bool isShowing {
        get { return gameObject.activeSelf; }
        set {
            if (isShowing == value) return;
            if (value) {
                gameObject.SetActive(true);
                GetComponent<RectTransform>().anchoredPosition = originalPosition; // 还原到初始位置
            } else {
                gameObject.SetActive(false);
            }
        }
    }
    
}

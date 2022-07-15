using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyForm : MonoBehaviour {
    public KeyCode key = KeyCode.None;
    public bool defaultShow = false;
    private Vector2 originalPosition;
    public void Awake() {
        // ����ʾ�������л���ʾ״̬��
        if (key != KeyCode.None) {
            InputM.AddKeyboardEvent(key, () => SwapShowing());
        }
        // �����ʼλ��
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    // ���û���Ĵ��ڵ���ʾ״̬
    public void SwapShowing() {
        FormM.currentForm = (FormM.currentForm == this ? null : this);
    }
    public bool isShowing {
        get { return gameObject.activeSelf; }
        set {
            if (isShowing == value) return;
            if (value) {
                gameObject.SetActive(true);
                GetComponent<RectTransform>().anchoredPosition = originalPosition; // ��ԭ����ʼλ��
            } else {
                gameObject.SetActive(false);
            }
        }
    }
    
}

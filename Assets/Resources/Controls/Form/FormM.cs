using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormM : MonoBehaviour {

    // 初始化窗口列表
    private static List<MyForm> formList = new();
    private void Start() {
        formList = new List<MyForm>(FindObjectsOfType<MyForm>());
        Debug.Log("找到了 " + formList.Count + " 个窗口");
        // 针对窗口的初始化状态，确保只有一个窗口正在显示中
        foreach (MyForm form in formList) {
            if (form.defaultShow) _currentForm = form;
        }
        foreach (MyForm form in formList) {
            form.isShowing = (_currentForm != null && _currentForm.name == form.name);
        }
    }
    
    // 管理当前窗口
    private static MyForm _currentForm = null;
    public static MyForm currentForm {
        get { return _currentForm; }
        set {
            if (value == _currentForm) return;
            _currentForm = value;
            // 激活或隐藏窗口
            foreach (MyForm form in formList) {
                form.isShowing = (value != null && value.name == form.name);
            }
        }
    }

}

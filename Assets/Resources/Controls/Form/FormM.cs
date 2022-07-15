using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormM : MonoBehaviour {

    // ��ʼ�������б�
    private static List<MyForm> formList = new();
    private void Start() {
        formList = new List<MyForm>(FindObjectsOfType<MyForm>());
        Debug.Log("�ҵ��� " + formList.Count + " ������");
        // ��Դ��ڵĳ�ʼ��״̬��ȷ��ֻ��һ������������ʾ��
        foreach (MyForm form in formList) {
            if (form.defaultShow) _currentForm = form;
        }
        foreach (MyForm form in formList) {
            form.isShowing = (_currentForm != null && _currentForm.name == form.name);
        }
    }
    
    // ����ǰ����
    private static MyForm _currentForm = null;
    public static MyForm currentForm {
        get { return _currentForm; }
        set {
            if (value == _currentForm) return;
            _currentForm = value;
            // ��������ش���
            foreach (MyForm form in formList) {
                form.isShowing = (value != null && value.name == form.name);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageM : MonoBehaviour {
    public static bool isChinese = true;
    private void Start() {
        DontDestroyOnLoad(gameObject);
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
        } else {
            GameObject.Find("FullScreenWarn1").SetActive(false);
            GameObject.Find("FullScreenWarn2").SetActive(false);
        }
    }
    public void SetIsChinese(bool value) { 
        isChinese = value;
        SceneManager.LoadScene("SceneMain");
    }

    private bool isInited = false;
    private void Update() {
        if (isInited) return;
        if (GameObject.Find("TextFire") == null) return;
        isInited = true;
        Debug.Log("�ѳ�ʼ������");

        GameObject.Find("TextFire").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "���" : "Shoot";
        GameObject.Find("TextReload").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "װ��" : "Reload";
        GameObject.Find("TextDeath").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "��" : "YOU ARE DEAD";
        GameObject.Find("TextWin").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "��" : "The End";
        GameObject.Find("TextWin (1)").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "��л�������" : "Thanks for playing";
        GameObject.Find("PanWin1").SetActive(false);
        Destroy(gameObject);
    }
}

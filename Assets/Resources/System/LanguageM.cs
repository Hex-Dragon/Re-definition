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
        Debug.Log("已初始化翻译");

        GameObject.Find("TextFire").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "射击" : "Shoot";
        GameObject.Find("TextReload").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "装弹" : "Reload";
        GameObject.Find("TextDeath").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "寄" : "YOU ARE DEAD";
        GameObject.Find("TextWin").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "完" : "The End";
        GameObject.Find("TextWin (1)").GetComponent<TMPro.TextMeshProUGUI>().text = isChinese ? "感谢你的游玩" : "Thanks for playing";
        GameObject.Find("PanWin1").SetActive(false);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class StoryM : MonoBehaviour {

    
    public TMPro.TextMeshProUGUI textStory;
    public RectTransform panStory;
    void Start() {
        StartCoroutine(test());
    }
    IEnumerator SetStoryText(string textCh, string textEn = "English", float hideTime = 2f) {
        textStory.text = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)" ? textCh : textEn;
        panStory.gameObject.SetActive(true);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(panStory);
        //panStory.GetComponent<ContentSizeMininum>().Update();
        yield return new WaitForSeconds(hideTime);
    }
    public void HideStoryText() {
        panStory.gameObject.SetActive(false);
    }

    IEnumerator test() {
        while (true) {
            yield return StartCoroutine(SetStoryText("���ǵ�һ�仰", "Eng" , 2f));
            yield return StartCoroutine(SetStoryText("���ǵڶ��仰���ı����С�ı�����"));
            HideStoryText();
            yield return new WaitForSeconds(2f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class StoryM : MonoBehaviour {

    
    public TMPro.TextMeshProUGUI textStory;
    public RectTransform shapeBack;
    void Start() {
        StartCoroutine(test());
    }
    IEnumerator SetStoryText(string textCh, string textEn = "English", float hideTime = 2f) {
        textStory.text = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)" ? textCh : textEn;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textStory.rectTransform);
        DOTween.Kill("StoryBackground");
        shapeBack.DOScaleX((textStory.rectTransform.rect.width + 40f) / 100f, 0.06f).SetId("StoryBackground");
        yield return new WaitForSeconds(hideTime);
    }
    public void HideStoryText() {
        textStory.text = "";
        shapeBack.DOScaleX(0, 0.06f);
    }

    IEnumerator test() {
        while (true) {
            yield return StartCoroutine(SetStoryText("这是第一句话", "Eng" , 2f));
            yield return StartCoroutine(SetStoryText("这是第二句话，文本框大小改变了吗？"));
            HideStoryText();
            yield return new WaitForSeconds(2f);
        }
    }

}

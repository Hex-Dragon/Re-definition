using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class StoryM : MonoBehaviour {

    // �ı�
    public TMPro.TextMeshProUGUI textStory;
    public RectTransform shapeBack;
    IEnumerator SetStoryText(string textCh, string textEn = "English", float hideTime = 2f) {
        textStory.text = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-CN)" ? textCh : textEn;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textStory.rectTransform);
        // ����
        DOTween.Kill("StoryFont");
        textStory.color = Color.black;
        textStory.DOColor(Color.white, 0.2f).SetId("StoryFont");
        DOTween.Kill("StoryBackground");
        shapeBack.DOScaleX((textStory.rectTransform.rect.width + 40f) / 100f, 0.1f).SetId("StoryBackground");
        yield return new WaitForSeconds(hideTime);
    }
    public void HideStoryText() {
        textStory.text = "";
        shapeBack.DOScaleX(0, 0.06f);
    }

    // ˢ��
    private Dictionary<Spawner.EnemyType, List<Spawner>> spawners = null;
    public void Spawn(Spawner.EnemyType type, int count = 1) {
        // ��ʼ��
        if (spawners == null) {
            spawners = new();
            foreach (Spawner spawner in FindObjectsOfType<Spawner>()) {
                if (spawners.ContainsKey(spawner.type)) {
                    spawners[spawner.type].Add(spawner);
                } else {
                    spawners[spawner.type] = new() { spawner };
                }
            }
        }
        // ʵ��ˢ��
        spawners[type].Shuffle();
        for (int i = 0; i < Mathf.Min(6, count); i++) {
            spawners[type][i].Spawn();
        }
    }

    void Start() {
        StartCoroutine(test());
    }
    IEnumerator test() {
        while (true) {
            Spawn(Spawner.EnemyType.Mover);
            yield return StartCoroutine(SetStoryText("��ˢһ��Ϲ���Ƶ�", "Eng" , 3f));
            Spawn(Spawner.EnemyType.Arrow, 3);
            yield return StartCoroutine(SetStoryText("��ˢ�������������ŵ�", "", 3f));
            Spawn(Spawner.EnemyType.Heavy);
            yield return StartCoroutine(SetStoryText("�����һ����Ӳ�ģ�����", "", 3f));
            HideStoryText();
            yield return new WaitForSeconds(5f);
            yield return StartCoroutine(SetStoryText("׼����һ�֡���", "", 3f));
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
[AddComponentMenu("UI/Content Size Mininum")]
public class ContentSizeMininum : MonoBehaviour {

    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform {
        get {
            if (m_Rect == null) m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    public RectTransform target;
    public void Update() {
        if (target == null) return;
        Vector2 newSize = new(LayoutUtility.GetPreferredWidth(target), LayoutUtility.GetPreferredHeight(target));
        if (rectTransform.sizeDelta != newSize) rectTransform.sizeDelta = newSize;
    }

}

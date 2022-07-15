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

    void Update() {
        List<RectTransform> childTransforms = new(GetComponentsInChildren<RectTransform>());
        if (childTransforms.Count == 1) return;
        RectTransform childTransform = childTransforms[1];
        Vector2 newSize = new(LayoutUtility.GetPreferredWidth(childTransform), LayoutUtility.GetPreferredHeight(childTransform));
        if (rectTransform.sizeDelta != newSize) rectTransform.sizeDelta = newSize;
    }

}

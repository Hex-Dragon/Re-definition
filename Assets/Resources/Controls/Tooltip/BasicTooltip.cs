using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Basic Tooltip")]
public class BasicTooltip : MonoBehaviour {
    private static BasicTooltipReceiver target;
    private static TextPic textTitle, textDescription;

    public string title;
    [TextArea] public string description;

    private RectTransform rectTransform;
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        if (target == null) {
            // ��ʼ���ؼ���ȡ
            target = GameObject.Find("TooltipBasic").GetComponent<BasicTooltipReceiver>();
            foreach (TextPic textMesh in target.GetComponentsInChildren<TextPic>()) {
                if (textMesh.name == "TextTooltipBasicTitle") textTitle = textMesh;
                if (textMesh.name == "TextTooltipBasicDescription") textDescription = textMesh;
            }
            // ���
            textTitle.text = ""; textTitle.gameObject.SetActive(false);
            textDescription.text = "";
        }
        Update();
    }

    private bool _hovered = false;
    private bool hovered {
        get { return _hovered; }
        set {
            if (_hovered == value) return;
            _hovered = value;
            if (value) {
                StartHover(UIExtensionsInputManager.MousePosition, true);
            } else {
                target.HideTooltip();
            }
        }
    }
    void Update() {
        if (Camera.current == null) return; // ��֪��Ϊɶ��갴�²��ƶ���ʱ�����һ˲���� Camera.current ��� null
        hovered = !ReorderableListElement.isAnythingDragging && // �϶��в���ʾ Tooltip
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.current);
        // ����ʾʱ���� Tooltip ����
        if (!hovered) return;
        if (textTitle.rawText != title) { // ֻ�����Ա仯ʱ����
            textTitle.text = title;
            textTitle.gameObject.SetActive(title != string.Empty); // �������Ϊ�վ�����
        }
        if (textDescription.rawText != description) {
            textDescription.text = description;
        }
        // ����λ��
        StartHover(UIExtensionsInputManager.MousePosition);
    }

    #region "ԭʼ Sender"

    /// <summary>
    /// This info is needed to make sure we make the necessary translations if the tooltip and this trigger are children of different space canvases
    /// </summary>
    private bool isChildOfOverlayCanvas = false;

    void Start() {
        //attempt to check if our canvas is overlay or not and check our "is overlay" accordingly
        Canvas ourCanvas = GetComponentInParent<Canvas>();
        if (ourCanvas && ourCanvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            isChildOfOverlayCanvas = true;
        }
    }

    /// <summary>
    /// Checks if the tooltip and the transform this trigger is attached to are children of differently-spaced Canvases
    /// </summary>
    public bool WorldToScreenIsRequired {
        get {
            return (isChildOfOverlayCanvas && target.guiMode == RenderMode.ScreenSpaceCamera) ||
                (!isChildOfOverlayCanvas && target.guiMode == RenderMode.ScreenSpaceOverlay);
        }
    }
    
    //public void OnDeselect(BaseEventData eventData) {
    //    hovered = false;
    //}

    void StartHover(Vector3 position, bool shouldCanvasUpdate = false) {
        target.SetTooltip(position, shouldCanvasUpdate);
    }

    #endregion

}

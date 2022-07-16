using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DiceUI : MonoBehaviour {

    private Image spriteBg, spriteLetter;
    private RectTransform rectTransform;
    public InputM.KeyType keybind;
    void Start() {
        spriteBg = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        spriteLetter = GetComponentsInChildren<Image>().Where(sp => sp.gameObject.name == "ImgText").First();
        UpdateLetter();
    }
    public void UpdateLetter() {
        spriteLetter.sprite = InputM.GetKeyTextSprite(InputM.GetKeyRaw(keybind));
    }

    private bool isKeyDown = false;
    void LateUpdate() {
        if (isKeyDown == InputM.GetKeyUI(keybind)) return;
        isKeyDown = !isKeyDown;
        UpdateColor();
    }
    public void UpdateColor() {
        spriteBg.DOColor(isKeyDown ?
            new Color(49f / 255f, 38f / 255f, 31f / 255f, isDropped ? 0.3f : 1f) :
            new Color(111f / 255f, 93f / 255f, 80f / 255f, isDropped ? 0.3f : 1f), 0.05f);
    }

    public bool allowPress = true;
    void Update() {
        if (!Input.GetMouseButtonUp(0) || !allowPress || 
            !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.allCameras[0])) return;
        // µã»÷
        DropDice();
    }

    public bool isDropped = false;
    public void DropDice() {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.allCameras[0], transform.position);
        Vector2 currentPos = Camera.allCameras[0].ScreenToWorldPoint(screenPos);
        DiceEntity diceNew = GameObject.Instantiate(InputM.instance.dicePrefab).GetComponent<DiceEntity>();
        diceNew.transform.position = currentPos;
        diceNew.key = keybind;
        isDropped = true;
        UpdateColor();
    }

}

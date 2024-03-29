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
            new Color(247f / 255f, 142f / 255f, 48f / 255f, isDropped ? 0.3f : 1f) :
            new Color(1f, 1f, 1f, isDropped ? 0.3f : 1f), 0.05f);
        spriteLetter.color = isDropped ? new Color(1f, 1f, 1f, 0.7f) : new Color(1f, 1f, 1f, 1f);
    }

    public bool allowPress = true;
    void Update() {
        if (!Input.GetMouseButtonUp(0) || !(allowPress || Input.GetKey(KeyCode.LeftControl)) || 
            !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, AspectUtility.cam)) return;
        // ���
        DropDice(DiceEntity.DiceType.Restore);
    }

    public bool isDropped = false;
    public void DropDice(DiceEntity.DiceType type) {
        DiceEntity diceNew = GameObject.Instantiate(InputM.instance.dicePrefab).GetComponent<DiceEntity>();
        diceNew.transform.localScale = Vector2.one * 1.11f;
        diceNew.transform.position = rectTransform.position;
        diceNew.key = keybind;
        diceNew.diceType = type;
        isDropped = true;
        UpdateColor();
    }

}

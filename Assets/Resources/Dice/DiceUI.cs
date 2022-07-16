using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DiceUI : MonoBehaviour {

    private Image spriteBg, spriteLetter;
    public InputM.KeyType keybind;
    void Start() {
        spriteBg = GetComponent<Image>();
        spriteLetter = GetComponentsInChildren<Image>().Where(sp => sp.gameObject.name == "ImgText").First();
        UpdateLetter();
    }
    public void UpdateLetter() {
        spriteLetter.sprite = InputM.GetKeyTextSprite(InputM.GetKeyRaw(keybind));
    }

    private bool isKeyDown = false;
    void Update() {
        if (isKeyDown == InputM.GetKeyUI(keybind)) return;
        isKeyDown = !isKeyDown;
        spriteBg.DOColor(isKeyDown ? new Color(49f / 255f, 38f / 255f, 31f / 255f, 1) : new Color(111f / 255f, 93f / 255f, 80f / 255f, 1), 0.05f);
    }
    
}

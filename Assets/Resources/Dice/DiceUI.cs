using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour {

    private Image sprite;
    public InputM.KeyType keybind;
    void Start() {
        sprite = GetComponent<Image>();
    }

    private bool isKeyDown = false;
    void Update() {
        if (isKeyDown == InputM.GetKeyUI(keybind)) return;
        isKeyDown = !isKeyDown;
        sprite.DOColor(isKeyDown ? new Color(49f / 255f, 38f / 255f, 31f / 255f, 1) : new Color(111f / 255f, 93f / 255f, 80f / 255f, 1), 0.05f);
    }
    
}

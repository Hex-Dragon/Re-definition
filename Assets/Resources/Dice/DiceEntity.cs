using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class DiceEntity : MonoBehaviour {

    public InputM.KeyType key;
    private string currentLetter;
    private SpriteRenderer spriteText;
    private void Start() {
        spriteText = GetComponentsInChildren<SpriteRenderer>().Where(sp => sp.gameObject.name == "ImgText").First();
        GetComponentsInChildren<SpriteRenderer>().Where(sp => sp.gameObject.name == "ImgType").First().sprite = InputM.GetKeyTypeSprite(key);
        currentLetter = InputM.GetKeyRaw(key);
        UpdateSpriteText();
    }
    private void UpdateSpriteText() => spriteText.sprite = InputM.GetKeyTextSprite(currentLetter);

    const float rollTime = 3.5f, endTime = 1.5f;
    public DiceType diceType = DiceType.NoRestore; public bool pickedUp = false;
    public enum DiceType { Restore, NoRestore, Bad }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player") || pickedUp) return;
        pickedUp = true;
        AudioM.Play("DicePickup1", 0.8f);
        // 关闭其他移动
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        // 开始旋转
        transform.DORotate(new Vector3(0, 0, 360f * Mathf.RoundToInt(rollTime * 1.5f)), rollTime, RotateMode.FastBeyond360);
        transform.DOScale(1, rollTime).SetRelative();
        transform.DOMoveY(transform.position.y + 2, rollTime);
        StartCoroutine(Roll());
    }
    IEnumerator Roll() {
        for (int i = 0; i < rollTime * 8; i++) {
            currentLetter = Modules.RandomOne("wasdlr".ToList()).ToString();
            UpdateSpriteText();
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < Mathf.RoundToInt(rollTime * 0.333f); i++) {
            currentLetter = Modules.RandomOne("wasdlr".ToList()).ToString();
            UpdateSpriteText();
            yield return new WaitForSeconds(0.333f);
        }
        // 最终确定
        string possible = InputM.GetPossibleResults(key, diceType);
        Debug.Log(possible);
        currentLetter = Modules.RandomOne(possible.ToCharArray().ToList()).ToString();
        UpdateSpriteText();
        yield return new WaitForSeconds(0.5f);
        // 强调动画
        AudioM.Play("DicePickup2");
        transform.DOScale(0.5f, 0.1f).SetRelative();
        yield return new WaitForSeconds(0.1f);
        transform.DOScale(-0.2f, 0.1f).SetRelative();
        yield return new WaitForSeconds(0.7f);
        // 结束动画
        transform.DOScale(-1.3f, endTime).SetRelative();
        GameObject endObj = InputM.GetDiceUI(key);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(AspectUtility.cam, endObj.transform.position);
        transform.DOMove((Vector2) AspectUtility.cam.ScreenToWorldPoint(screenPos), endTime);
        yield return new WaitForSeconds(endTime);
        // 设置
        InputM.SetKey(key, currentLetter);
        endObj.GetComponent<DiceUI>().isDropped = false;
        endObj.GetComponent<DiceUI>().UpdateColor();
        yield return new WaitForSeconds(0.1f);
        // 销毁
        Modules.DestroyGameObject(gameObject);
    }


}
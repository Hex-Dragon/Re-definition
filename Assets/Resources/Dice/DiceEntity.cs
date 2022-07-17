using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class DiceEntity : MonoBehaviour
{

    public InputM.KeyType key;
    private string currentLetter;
    private SpriteRenderer spriteText;
    private void Start()
    {
        spriteText = GetComponentsInChildren<SpriteRenderer>().Where(sp => sp.gameObject.name == "ImgText").First();
        GetComponentsInChildren<SpriteRenderer>().Where(sp => sp.gameObject.name == "ImgType").First().sprite = InputM.GetKeyTypeSprite(key);
        currentLetter = InputM.GetKeyRaw(key);
        UpdateSpriteText();
    }
    private void UpdateSpriteText() => spriteText.sprite = InputM.GetKeyTextSprite(currentLetter);

    const float rollTime = 3.5f, endTime = 1.5f;
    public bool canRestore = true;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        AudioM.Play("DicePickup1");
        // 关闭其他移动
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        // 开始旋转
        transform.DORotate(new Vector3(0, 0, 360f * Mathf.RoundToInt(rollTime * 1.5f)), rollTime, RotateMode.FastBeyond360);
        transform.DOScale(2, rollTime);
        transform.DOMoveY(transform.position.y + 3, rollTime);
        StartCoroutine(Roll());
    }
    IEnumerator Roll()
    {
        for (int i = 0; i < rollTime * 8; i++)
        {
            currentLetter = Modules.RandomOne("wasdlr".ToList()).ToString();
            UpdateSpriteText();
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < Mathf.RoundToInt(rollTime * 0.333f); i++)
        {
            currentLetter = Modules.RandomOne("wasdlr".ToList()).ToString();
            UpdateSpriteText();
            yield return new WaitForSeconds(0.333f);
        }
        // 最终确定
        string possible = InputM.GetPossibleResults(key, canRestore);
        Debug.Log(possible);
        currentLetter = Modules.RandomOne(possible.ToCharArray().ToList()).ToString();
        UpdateSpriteText();
        yield return new WaitForSeconds(0.5f);
        // 强调动画
        AudioM.Play("DicePickup2");
        transform.DOScale(2.5f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        transform.DOScale(2.3f, 0.1f);
        yield return new WaitForSeconds(0.7f);
        // 结束动画
        transform.DOScale(1, endTime);
        GameObject endObj = InputM.GetDiceUI(key);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.allCameras[0], endObj.transform.position);
        transform.DOMove((Vector2)Camera.allCameras[0].ScreenToWorldPoint(screenPos), endTime);
        yield return new WaitForSeconds(endTime);
        // 设置
        InputM.SetKey(key, currentLetter);
        endObj.GetComponent<DiceUI>().isDropped = false;
        endObj.GetComponent<DiceUI>().UpdateColor();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }


}
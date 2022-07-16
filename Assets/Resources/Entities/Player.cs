using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : EntityBase {

    internal override bool isPressingCrouch() => InputM.GetKeyEvent(InputM.KeyType.Crouch);
    internal override bool isPressDownJump() => InputM.GetKeyEvent(InputM.KeyType.Jump);
    internal override bool isPressingLeft() => InputM.GetKeyEvent(InputM.KeyType.Left);
    internal override bool isPressingRight() => InputM.GetKeyEvent(InputM.KeyType.Right);

    // ����
    public RectTransform[] hearts;
    private int _hp = 3;
    private int hp {
        get { return _hp; }
        set {
            if (_hp == value) return;
            _hp = value;
            if (_hp == 0) _hp = 3;
            RefreshHearts();
        }
    }
    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitBorder() {
        Hurt();
        transform.position = new Vector3(0, 8, 0);
    }
    internal override void OnHitEnemy(Collision2D collision) {
        Hurt();
        int face = collision.transform.position.x < transform.position.x ? 1 : -1;
        rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
    }
    public void Hurt() {
        hp--;
        AudioM.Play("hurt");
        rb.velocity = Vector2.zero;
        DOTween.Kill("ScreenShake");
        Camera.current.DOShakePosition(0.3f, 0.3f, 20).SetId("ScreenShake");
    }

    // ����
    public float healTime = 4f; private float currentHealTime = 0f;
    private void RefreshHearts() {
        float totalHp = Mathf.Clamp(hp + Mathf.Pow(currentHealTime / healTime, 7f) * 0.7f + currentHealTime / healTime * 0.3f, 0, 3);
        for (int i = 0; i < 3; i++) {
            hearts[i].localScale = Vector3.one * Mathf.Clamp(totalHp - i, 0, 1);
        }
    }

    // ���
    public int maxBullet = 20, currentBullet = 20;
    public float shootDelay = 0.15f; private float currentShootDelay = 0f;
    public float reloadDelay = 1.5f; private float currentReloadDelay = 0f;
    public float bulletSpeed = 50f, bulletKnockback = 1f;
    public GameObject bullet; public TMPro.TextMeshProUGUI textBullet;
    internal override void OnUpdate() {
        // ����
        if (isPressingCrouch() && hp < 3 && isLand) {
            currentHealTime += Time.deltaTime;
            if (currentHealTime > healTime) {
                currentHealTime -= healTime;
                hp++;
            }
            RefreshHearts();
        } else if (currentHealTime != 0f) {
            currentHealTime = 0f;
            RefreshHearts();
        }
        // װ��
        if (InputM.GetKeyEvent(InputM.KeyType.Reload) && currentBullet < maxBullet && currentReloadDelay <= 0f) {
            AudioM.Play("reload");
            currentReloadDelay = reloadDelay;
        } else if (currentReloadDelay > 0 && currentReloadDelay < Time.deltaTime) {
            currentReloadDelay = 0f; currentBullet = maxBullet;
        } else if (currentReloadDelay > 0) {
            currentReloadDelay -= Time.deltaTime;
        }
        // ����
        if (InputM.GetKeyEvent(InputM.KeyType.Fire) && currentBullet > 0 && currentShootDelay <= 0f && currentReloadDelay <= 0f) {
            AudioM.Play("fire");
            currentBullet--; currentShootDelay = shootDelay;
            Vector2 fromPos = transform.position + Vector3.up * 0.55f;
            Vector2 toPos = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootVector = (toPos - fromPos).normalized;
            GameObject newBullet = Instantiate(bullet, fromPos, Quaternion.identity);
            newBullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * shootVector;
            newBullet.transform.eulerAngles = (shootVector.x > 0 ? -1 : 1) * Vector2.Angle(shootVector, Vector2.up) * Vector3.forward;
            // ������
            rb.velocity += bulletKnockback * -shootVector;
        } else if (currentShootDelay > 0) {
            currentShootDelay -= Time.deltaTime;
        }
        // ���� UI
        textBullet.text = currentReloadDelay > 0f ?
            "".PadLeft(maxBullet - Mathf.RoundToInt(currentReloadDelay / reloadDelay * maxBullet), "|".ToCharArray().First()) : 
            "".PadLeft(currentBullet, "|".ToCharArray().First());
        textBullet.color = currentReloadDelay > 0f ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.6588235f, 1f, 0.6941177f);
    }

}

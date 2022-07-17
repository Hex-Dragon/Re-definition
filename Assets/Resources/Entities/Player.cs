using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : EntityBase {
    public static Player instance;
    private void Awake() {
        instance = this;
    }

    internal override bool isPressingCrouch() => InputM.GetKeyEvent(InputM.KeyType.Crouch);
    internal override bool isPressDownJump() => InputM.GetKeyEvent(InputM.KeyType.Jump);
    internal override bool isPressingLeft() => InputM.GetKeyEvent(InputM.KeyType.Left);
    internal override bool isPressingRight() => InputM.GetKeyEvent(InputM.KeyType.Right);

    // 生命
    public RectTransform[] hearts;
    private static int _hp = 3;
    public static int hp {
        get { return _hp; }
        set {
            if (_hp == value) return;
            _hp = value;
            instance.RefreshHearts();
        }
    }
    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitBorder() {
        Hurt();
        resistanceTime = resistanceOnHurt;
        transform.position = new Vector3(-6, 10, 0);
    }
    internal override void OnHitEnemy(Collision2D collision) {
        if (Hurt()) {
            int face = collision.transform.position.x < transform.position.x ? 1 : -1;
            rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
        }
    }
    public float resistanceOnHurt = 1f; private float resistanceTime = 0f;
    public bool Hurt() {
        if (resistanceTime <= 0.001f) {
            resistanceTime = resistanceOnHurt;
            hp--;
            AudioM.Play("hurt", 0.6f);
            rb.velocity = Vector2.zero;
            DOTween.Kill("ScreenShake");
            Camera.current.DOShakePosition(0.4f, 0.5f, 25).SetId("ScreenShake");
            return true;
        } else {
            return false;
        }
    }

    // 治疗
    public float healTime = 4f; private float currentHealTime = 0f;
    private void RefreshHearts() {
        float totalHp = Mathf.Clamp(hp + Mathf.Pow(currentHealTime / healTime, 7f) * 0.7f + currentHealTime / healTime * 0.3f, 0, 3);
        for (int i = 0; i < 3; i++) {
            hearts[i].localScale = Vector3.one * Mathf.Clamp(totalHp - i, 0, 1);
        }
    }

    // 射击
    public int maxBullet = 20, currentBullet = 20;
    public float shootDelay = 0.15f; private float currentShootDelay = 0f;
    public float reloadDelay = 1.5f; private float currentReloadDelay = 0f;
    public float bulletSpeed = 50f, bulletKnockback = 1f;
    public GameObject bullet; public TMPro.TextMeshProUGUI textBullet;
    internal override void OnUpdate() {
        // 无敌
        if (resistanceTime > 0f) resistanceTime -= Time.deltaTime;
        // 治疗
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
        // 装填
        if (InputM.GetKeyEvent(InputM.KeyType.Reload) && currentBullet < maxBullet && currentReloadDelay <= 0f) {
            AudioM.Play("reload", 0.8f);
            currentReloadDelay = reloadDelay;
        } else if (currentReloadDelay > 0 && currentReloadDelay < Time.deltaTime) {
            currentReloadDelay = 0f; currentBullet = maxBullet;
        } else if (currentReloadDelay > 0) {
            currentReloadDelay -= Time.deltaTime;
        }
        // 开火
        if (InputM.GetKeyEvent(InputM.KeyType.Fire) && currentBullet > 0 && currentShootDelay <= 0f && currentReloadDelay <= 0f) {
            AudioM.Play("fire", 0.25f);
            currentBullet--; currentShootDelay = shootDelay;
            Vector2 fromPos = transform.position + Vector3.up * ((canCrouch && isPressingCrouch()) ? 0.55f : 1.3f);
            Vector2 toPos = AspectUtility.cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootVector = (toPos - fromPos).normalized;
            GameObject newBullet = Instantiate(bullet, fromPos, Quaternion.identity);
            newBullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * shootVector;
            newBullet.transform.eulerAngles = (shootVector.x > 0 ? -1 : 1) * Vector2.Angle(shootVector, Vector2.up) * Vector3.forward;
            // 后座力
            rb.velocity += bulletKnockback * -shootVector;
        } else if (currentShootDelay > 0) {
            currentShootDelay -= Time.deltaTime;
        }
        // 更新 UI
        textBullet.text = currentReloadDelay > 0f ?
            "".PadLeft(maxBullet - Mathf.RoundToInt(currentReloadDelay / reloadDelay * maxBullet), "|".ToCharArray().First()) :
            "".PadLeft(currentBullet, "|".ToCharArray().First());
        textBullet.color = currentReloadDelay > 0f ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.9686275f, 0.5568628f, 0.1098039f);
    }

}
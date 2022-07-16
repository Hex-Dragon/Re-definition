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

    internal override void OnHitBorder() {
        Hurt();
        transform.position = new Vector3(0, 8, 0);
    }
    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitEnemy(Collision2D collision) {
        Hurt();
        int face = collision.transform.position.x < transform.position.x ? 1 : -1;
        rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
    }
    public void Hurt() {
        AudioM.Play("hurt");
        rb.velocity = Vector2.zero;
        DOTween.Kill("ScreenShake");
        Camera.current.DOShakePosition(0.3f, 0.3f, 20).SetId("ScreenShake");
    }

    // ���
    public int maxBullet = 20, currentBullet = 20;
    public float shootDelay = 0.15f; private float currentShootDelay = 0f;
    public float reloadDelay = 1.5f; private float currentReloadDelay = 0f;
    public float bulletSpeed = 50f, bulletKnockback = 1f;
    public GameObject bullet;
    internal override void OnUpdate() {
        // װ��
        if (InputM.GetKeyEvent(InputM.KeyType.Reload) && currentBullet < maxBullet && currentReloadDelay <= 0f) {
            // TODO: װ����Ч����ʱ��Ϊ reloadDelay���� Unity ��� Player �ұ�������������ֿ����ǹ��ڵģ�
            currentReloadDelay = reloadDelay;
        } else if (currentReloadDelay > 0 && currentReloadDelay < Time.deltaTime) {
            currentReloadDelay = 0f; currentBullet = maxBullet;
        } else if (currentReloadDelay > 0) {
            currentReloadDelay -= Time.deltaTime;
        }
        // ����
        if (InputM.GetKeyEvent(InputM.KeyType.Fire) && currentBullet > 0 && currentShootDelay <= 0f) {
            // TODO: ������Ч
            currentBullet--; currentShootDelay = shootDelay;
            Vector2 fromPos = transform.position + Vector3.up * 0.55f;
            Vector2 toPos = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootVector = (toPos - fromPos).normalized;
            GameObject newBullet = Instantiate(bullet, fromPos, Quaternion.FromToRotation(fromPos, toPos));
            newBullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * shootVector;
            // ������
            rb.velocity += bulletKnockback * -shootVector;
        } else if (currentShootDelay > 0) {
            currentShootDelay -= Time.deltaTime;
        }
    }

}

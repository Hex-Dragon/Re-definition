using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : EntityBase {

    internal override void OnHitBorder() {
        Destroy(gameObject);
    }

    internal override void OnHitBullet() {
        // ���˶���
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.04f).SetLoops(2, LoopType.Yoyo);
        // �����ж�
        hp--;
        if (hp < 0) {
            //TODO: ����������Ч
            transform.localScale = Vector3.zero;
            Destroy(gameObject, 1); // ��ֹ����û�����굼�±���
        }
    }

}

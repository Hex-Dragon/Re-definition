using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : EntityBase {

    internal override void OnHitBorder() {
        Destroy(gameObject);
    }

    internal override void OnHitBullet() {
        // 受伤动画
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.04f).SetLoops(2, LoopType.Yoyo);
        // 死亡判断
        hp--;
        if (hp < 0) {
            //TODO: 敌人死亡音效
            transform.localScale = Vector3.zero;
            Destroy(gameObject, 1); // 防止动画没播放完导致报错
        }
    }

}

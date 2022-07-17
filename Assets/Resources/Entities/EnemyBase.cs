using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : EntityBase {

    public bool movingLeft = false;
    internal override void OnHitBorder() {
        Destroy(gameObject);
    }

    public int hp; internal int currentHp = -1;
    internal override void OnHitBullet() {
        // 受伤动画
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.04f).SetLoops(2, LoopType.Yoyo);
        // 死亡判断
        if (currentHp == -1) currentHp = hp;
        currentHp--;
        if (currentHp < 0) {
            AudioM.Play("enemy_die");
            transform.localScale = Vector3.zero;
            Destroy(gameObject, 1); // 防止动画没播放完导致报错
        }
    }

}

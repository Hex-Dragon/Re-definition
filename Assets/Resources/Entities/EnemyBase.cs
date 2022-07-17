using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : EntityBase {

    internal override void OnUpdate() {
        rb.mass = baseMass * (towardsRight ? 1.2f : 0.8f);
        // ∑¥œÚ
        if (towardsRight != (Mathf.Abs(rb.velocity.x) > filpSpeed ? rb.velocity.x > 0 : towardsRight)) {
            towardsRight = !towardsRight;
            transform.DOScaleX(towardsRight ? 1 : -1, 0.15f);
        }
    }

    public bool movingLeft = false;
    internal override void OnHitBorder() {
        // À¿Õˆ
        Modules.DestroyGameObject(gameObject);
    }

    public int hp; internal int currentHp = -10;
    internal override void OnHitBullet() {
        //  ‹…À∂Øª≠
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.04f).SetLoops(2, LoopType.Yoyo);
        // À¿Õˆ≈–∂œ
        if (currentHp == -10) currentHp = hp;
        currentHp--;
        if (currentHp <= 0) {
            AudioM.Play("enemy_die", 0.7f);
            Modules.DestroyGameObject(gameObject);
            if (hp > 3) { Camera.current.DOShakePosition(0.15f, 0.4f, 100).SetId("ScreenShake"); }
        }
    }

}

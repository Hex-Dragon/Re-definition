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
        transform.position = new Vector3(1000, 1000);
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    public int hp; internal int currentHp = -1;
    internal override void OnHitBullet() {
        //  ‹…À∂Øª≠
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.04f).SetLoops(2, LoopType.Yoyo);
        // À¿Õˆ≈–∂œ
        if (currentHp == -1) currentHp = hp;
        currentHp--;
        if (currentHp <= 0) {
            AudioM.Play("enemy_die", 0.5f);
            OnHitBorder();
        }
    }

}

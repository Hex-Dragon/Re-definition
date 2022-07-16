using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavy : EnemyBase {

    public bool movingLeft = false;
    internal override bool isPressingCrouch() => false;
    internal override bool isPressDownJump() => false;
    internal override bool isPressingLeft() => movingLeft;
    internal override bool isPressingRight() => !movingLeft;
    //internal override void OnUpdate() {
    //    bool shouldTurn = false;
    //    if (Mathf.Abs(rb.velocity.x) > 0.001f && (rb.velocity.x > 0 == movingLeft)) shouldTurn = true;
    //    if (shouldTurn) movingLeft = !movingLeft;
    //}

    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitEnemy(Collision2D collision) {
        int face = collision.transform.position.x < transform.position.x ? 1 : -1;
        rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
    }

}

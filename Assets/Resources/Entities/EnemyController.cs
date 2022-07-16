using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityBase {
    
    internal override bool isPressingCrouch() => false;
    internal override bool isPressingJump() => false;
    internal override bool isPressingLeft() => false;
    internal override bool isPressingRight() => true;

    internal override void OnHitBorder() {
        Destroy(gameObject);
    }
    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitEnemy(Collision2D collision) {
        int face = collision.transform.position.x < transform.position.x ? 1 : -1;
        rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
    }

}

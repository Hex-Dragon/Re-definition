using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : EnemyBase {

    internal override bool isPressingCrouch() => false;
    internal override bool isPressDownJump() => false;
    internal override bool isPressingLeft() => movingLeft;
    internal override bool isPressingRight() => !movingLeft;

    private float turnCooldown = 0f;
    internal override void OnUpdate() {
        bool shouldTurn =
            (movingLeft && Physics2D.Linecast(transform.position, transform.position + new Vector3(-0.7f, 0.5f, 0f), LayerMask.GetMask("Marker"))) ||
            (!movingLeft && Physics2D.Linecast(transform.position, transform.position + new Vector3(0.7f, 0.5f, 0f), LayerMask.GetMask("Marker")));
        if (Mathf.Abs(rb.velocity.x) > 0.0001f && (rb.velocity.x > 0 == movingLeft)) shouldTurn = true;
        turnCooldown -= Time.deltaTime;
        if (shouldTurn && turnCooldown <= 0f) {
            movingLeft = !movingLeft;
            turnCooldown = 1f;
        }
    }

    public float knockbackForceH = 30f, knockbackForceV = 15f;
    internal override void OnHitEnemy(Collision2D collision) {
        int face = collision.transform.position.x < transform.position.x ? 1 : -1;
        rb.velocity = new Vector2(face * knockbackForceH, knockbackForceV);
    }

}

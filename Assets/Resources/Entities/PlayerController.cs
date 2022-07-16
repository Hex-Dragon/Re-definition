using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityBase {

    public string keyLeft = "a", keyRight = "d", keyJump = "w", keyDown = "s";
    internal override bool isPressingCrouch() => Input.GetKey(keyDown);
    internal override bool isPressingJump() => Input.GetKey(keyJump);
    internal override bool isPressingLeft() => Input.GetKey(keyLeft);
    internal override bool isPressingRight() => Input.GetKey(keyRight);

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
        rb.velocity = Vector2.zero;
    }

}

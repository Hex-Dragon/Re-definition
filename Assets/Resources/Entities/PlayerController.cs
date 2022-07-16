using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityBase {

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
        rb.velocity = Vector2.zero;
        Camera.current.DOShakePosition(0.3f, 0.3f, 20);
    }

}

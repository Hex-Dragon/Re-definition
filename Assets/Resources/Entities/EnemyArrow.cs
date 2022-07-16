using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : EntityBase {

    public bool movingLeft = false;
    internal override bool isPressingCrouch() => false;
    internal override bool isPressDownJump() => false;
    internal override bool isPressingLeft() => movingLeft;
    internal override bool isPressingRight() => !movingLeft;

    internal override void OnHitBorder() {
        Destroy(gameObject);
    }
    internal override void OnHitEnemy(Collision2D collision) {
        Destroy(gameObject);
    }

}

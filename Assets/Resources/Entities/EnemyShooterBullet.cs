using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterBullet : EnemyBase {

    internal override bool isPressingCrouch() => false;
    internal override bool isPressDownJump() => false;
    internal override bool isPressingLeft() => movingLeft;
    internal override bool isPressingRight() => !movingLeft;

    internal override void OnHitEnemy(Collision2D collision) {
        Destroy(gameObject);
    }

}
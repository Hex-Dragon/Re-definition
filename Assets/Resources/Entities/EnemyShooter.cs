using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : EnemyBase {

    internal override bool isPressingCrouch() => false;
    internal override bool isPressDownJump() => false;
    internal override bool isPressingLeft() => false;
    internal override bool isPressingRight() => false;
    internal override void OnHitEnemy(Collision2D collision) {}

    public float shootDelay = 5f, bulletSpeed = 1f; private float shootCooldown = 0f;
    public GameObject bullet;
    internal override void OnUpdate() {
        base.OnUpdate();
        if (!isLand) return;
        shootCooldown += Time.deltaTime;
        if (shootCooldown < shootDelay) return;
        // Éú³É×Óµ¯
        AudioM.Play("enemy_shoot", 0.5f); 
        shootCooldown = shootDelay * ((float) Modules.randomDefault.NextDouble() * 0.4f - 0.2f);
        GameObject newBullet = Instantiate(bullet, transform);
        newBullet.transform.position += new Vector3(0.5f, -0.5f, 0);
        newBullet.transform.SetParent(null);
        newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2((float) Modules.randomDefault.NextDouble() - 0.5f, 0.25f).normalized * bulletSpeed;
    }


}

using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour {
    internal Rigidbody2D rb;
    internal SpriteRenderer sprite;
    internal BoxCollider2D coll;
    public Sprite spriteNormal, spriteDown;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    public bool canCrouch = false;
    public float movementSpeed = 10f, jumpForce = 20f, horizontalF = 0.9f, horizontalA = 0.3f, jumpThreshold = 0.1f, downA = 0.3f, fallA = 0.1f;
    public float jumpBufferTime = 0.2f, jumpWolfTime = 0.2f; internal float jumpBuffer = 0f, jumpWolf = 0f, jumping = 0f;
    private bool towardsRight = true;
    void Update() {
        OnUpdate();
        // 获取当前情况
        float speedX = rb.velocity.x, speedY = rb.velocity.y;
        bool isCrouch = isPressingCrouch();
        bool isLand = Physics2D.OverlapCircle(transform.position + Vector3.left * 0.1f, jumpThreshold, LayerMask.GetMask("Tilemap")) || 
            Physics2D.OverlapCircle(transform.position + Vector3.left * -0.1f, jumpThreshold, LayerMask.GetMask("Tilemap"));
        float moveHorizontal = (isCrouch && isLand ? 0.35f : 1) * // 下蹲减速
            (isPressingLeft() == isPressingRight() ? 0 : // 没有同时按住左右
            (isPressingLeft() ? -1 : 1)); // 获取方向
        // 判断跳跃
        jumpBuffer -= Time.deltaTime; jumpWolf -= Time.deltaTime; jumping -= Time.deltaTime;
        if (isPressDownJump()) jumpBuffer = jumpBufferTime;
        if (isLand) jumpWolf = jumpWolfTime;
        if (jumpBuffer > 0 && jumpWolf > 0 && jumping < 0) {
            // 播放音效
            AudioM.Play("jump");

            speedY = jumpForce;
            jumpBuffer = 0;
            jumping = 0.5f;
        } else if (isCrouch) {
            speedY -= downA;
        }
        // 加速下落
        if (speedY < 0) speedY -= fallA;
        // 横向移动
        if (moveHorizontal != 0) {
            speedX = speedX * (1 - horizontalA) + movementSpeed * horizontalA * moveHorizontal;
        }
        speedX *= horizontalF;
        // 蹲下
        if (canCrouch) {
            if (isCrouch) {
                sprite.sprite = spriteDown;
                coll.size = new Vector2(coll.size.x, 0.4f);
                coll.offset = new Vector2(0, 0.3f);
            } else {
                sprite.sprite = spriteNormal;
                coll.size = new Vector2(coll.size.x, 1.2f);
                coll.offset = new Vector2(0, 0.7f);
            }
        }
        // 设置
        rb.velocity = new Vector2(speedX, speedY);
        if (towardsRight != (Mathf.Abs(speedX) > 0.01 ? speedX > 0 : towardsRight)) {
            // 反向
            towardsRight = !towardsRight;
            transform.DOScaleX(towardsRight ? 1 : -1, 0.1f);
        }
    }
    internal abstract bool isPressingLeft();
    internal abstract bool isPressingRight();
    internal abstract bool isPressDownJump();
    internal abstract bool isPressingCrouch();
    internal virtual void OnUpdate() {}

    // 碰撞检测
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Border")) OnHitBorder();
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player")) OnHitEnemy(collision);
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy")) OnHitEnemy(collision);
    }
    internal abstract void OnHitBorder();
    internal abstract void OnHitEnemy(Collision2D collision);

}

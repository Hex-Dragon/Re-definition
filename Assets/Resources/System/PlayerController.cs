using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    public LayerMask groundLayer;
    public Sprite spriteNormal, spriteDown;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    public string keyLeft = "a", keyRight = "d", keyJump = "w", keyDown = "s";
    public float movementSpeed = 10f, jumpForce = 20f, horizontalF = 0.9f, horizontalA = 0.3f, jumpThreshold = 0.1f, downA = 0.3f;
    public float jumpBufferTime = 0.2f, jumpWolfTime = 0.2f; private float jumpBuffer = 0f, jumpWolf = 0f, jumping = 0f;
    void Update() {
        // 获取当前情况
        float speedX = rb.velocity.x, speedY = rb.velocity.y;
        bool isDown = Input.GetKey(keyDown);
        bool isGround = Physics2D.OverlapCircle(transform.position, jumpThreshold, groundLayer);
        int moveHorizontal = isDown ? 0 : // 没有下蹲
            (Input.GetKey(keyLeft) == Input.GetKey(keyRight) ? 0 : // 没有同时按住左右
            (Input.GetKey(keyLeft) ? -1 : 1)); // 获取方向
        // 判断跳跃
        jumpBuffer -= Time.deltaTime; jumpWolf -= Time.deltaTime; jumping -= Time.deltaTime;
        if (Input.GetKey(keyJump)) jumpBuffer = jumpBufferTime;
        if (isGround) jumpWolf = jumpWolfTime;
        if (!isDown && jumpBuffer > 0 && jumpWolf > 0 && jumping < 0) { // 在地面、没有蹲下
            speedY = jumpForce;
            jumpBuffer = 0;
            jumping = 0.5f;
        } else if (isDown) {
            speedY -= downA;
            if (speedY > 0) speedY = 0;
        }
        // 横向移动
        if (moveHorizontal != 0) {
            speedX = speedX * (1 - horizontalA) + movementSpeed * horizontalA * moveHorizontal;
        }
        speedX *= horizontalF;
        // 蹲下
        if (isDown) {
            sprite.sprite = spriteDown;
            coll.size = new Vector2(coll.size.x, 0.5f);
            coll.offset = new Vector2(0, 0.35f);
        } else {
            sprite.sprite = spriteNormal;
            coll.size = new Vector2(coll.size.x, 1.2f);
            coll.offset = new Vector2(0, 0.7f);
        }
        // 设置
        rb.velocity = new Vector2(speedX, speedY);
    }

}

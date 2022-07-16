using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    public LayerMask groundLayer;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public string keyLeft = "a", keyRight = "d", keyJump = "w", keyDown = "s";
    public float movementSpeed = 10f, jumpForce = 20f, horizontalF = 0.9f, horizontalA = 0.3f, jumpThreshold = 0.1f, jumpDetectHeight = 0.8f;
    public float jumpBufferTime = 0.2f, jumpWolfTime = 0.2f; private float jumpBuffer = 0f, jumpWolf = 0f;
    void FixedUpdate() {
        // 获取当前情况
        float speedX = rb.velocity.x, speedY = rb.velocity.y;
        bool isGround = Physics2D.OverlapCircle(transform.position - new Vector3(0, jumpDetectHeight, 0), jumpThreshold, groundLayer);
        bool isDown = Input.GetKey(keyDown) && isGround;
        int moveHorizontal = isDown ? 0 : // 没有下蹲
            (Input.GetKey(keyLeft) == Input.GetKey(keyRight) ? 0 : // 没有同时按住左右
            (Input.GetKey(keyLeft) ? -1 : 1)); // 获取方向
        // 判断跳跃
        jumpBuffer -= Time.fixedDeltaTime;
        if (Input.GetKeyDown(keyJump)) jumpBuffer = jumpBufferTime;
        jumpWolf -= Time.fixedDeltaTime;
        if (isGround) jumpWolf = jumpWolfTime;
        if (!isDown && jumpBuffer > 0 && jumpWolf > 0) { // 在地面、没有蹲下
            speedY = jumpForce;
            jumpBuffer = 0;
        }
        // 横向移动
        if (moveHorizontal != 0) {
            speedX = speedX * (1 - horizontalA) + movementSpeed * horizontalA * moveHorizontal;
        }
        speedX *= horizontalF;
        // 设置
        rb.velocity = new Vector2(speedX, speedY);
    }

}

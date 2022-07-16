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

    public float movementSpeed = 10f, jumpForce = 20f, horizontalF = 0.9f, horizontalA = 0.3f, jumpThreshold = 0.1f, downA = 0.3f, fallA = 0.1f;
    public float jumpBufferTime = 0.2f, jumpWolfTime = 0.2f; internal float jumpBuffer = 0f, jumpWolf = 0f, jumping = 0f;
    void Update() {
        // ��ȡ��ǰ���
        float speedX = rb.velocity.x, speedY = rb.velocity.y;
        bool isCrouch = isPressingCrouch();
        bool isLand = Physics2D.OverlapCircle(transform.position, jumpThreshold, LayerMask.GetMask("Tilemap"));
        int moveHorizontal = isCrouch ? 0 : // û���¶�
            (isPressingLeft() == isPressingRight() ? 0 : // û��ͬʱ��ס����
            (isPressingLeft() ? -1 : 1)); // ��ȡ����
        // �ж���Ծ
        jumpBuffer -= Time.deltaTime; jumpWolf -= Time.deltaTime; jumping -= Time.deltaTime;
        if (isPressingJump()) jumpBuffer = jumpBufferTime;
        if (isLand) jumpWolf = jumpWolfTime;
        if (!isCrouch && jumpBuffer > 0 && jumpWolf > 0 && jumping < 0) { // �ڵ��桢û�ж���
            speedY = jumpForce;
            jumpBuffer = 0;
            jumping = 0.5f;
        } else if (isCrouch) {
            speedY -= downA;
            if (speedY > 0) speedY = 0;
        }
        // ��������
        if (speedY < 0) speedY -= fallA;
        // �����ƶ�
        if (moveHorizontal != 0) {
            speedX = speedX * (1 - horizontalA) + movementSpeed * horizontalA * moveHorizontal;
        }
        speedX *= horizontalF;
        // ����
        if (isCrouch) {
            sprite.sprite = spriteDown;
            coll.size = new Vector2(coll.size.x, 0.4f);
            coll.offset = new Vector2(0, 0.3f);
        } else {
            sprite.sprite = spriteNormal;
            coll.size = new Vector2(coll.size.x, 1.2f);
            coll.offset = new Vector2(0, 0.7f);
        }
        // ����
        rb.velocity = new Vector2(speedX, speedY);
    }
    internal abstract bool isPressingLeft();
    internal abstract bool isPressingRight();
    internal abstract bool isPressingJump();
    internal abstract bool isPressingCrouch();

    // ��ײ���
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Border")) OnHitBorder();
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player")) OnHitEnemy(collision);
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy")) OnHitEnemy(collision);
    }
    internal abstract void OnHitBorder();
    internal abstract void OnHitEnemy(Collision2D collision);

}

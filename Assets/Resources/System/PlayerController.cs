using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float movementSpeed = 5f;

    private Rigidbody2D rb;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        // ÒÆ¶¯
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed, Input.GetAxis("Vertical") * movementSpeed);
    }

}

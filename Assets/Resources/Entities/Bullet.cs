using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Border")) AudioM.Play("bullet_explode", 0.2f);
        transform.localScale = Vector3.zero; Destroy(gameObject, 1);
    }

}

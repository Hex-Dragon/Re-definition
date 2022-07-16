using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        //TODO: ×Óµ¯±¬Õ¨ÒôÐ§£¨Ïú»Ù£©
        Destroy(gameObject);
    }

}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject bulletHit;
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Border")) {
            AudioM.Play("bullet_explode", 0.2f);
            var newBulletHit = GameObject.Instantiate(bulletHit);
            newBulletHit.transform.position = collision.contacts[0].point;
            newBulletHit.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InCirc)
                .onComplete += () => { Modules.DestroyGameObject(newBulletHit); };
        }
        Modules.DestroyGameObject(gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject prefeb;
    public EnemyType type;
    public enum EnemyType { Arrow, Mover, Heavy, Shooter }
    public GameObject parent;
    public void Spawn() {
        GameObject obj = Instantiate(prefeb, transform);
        obj.transform.parent = parent.transform;
        obj.GetComponent<EnemyBase>().movingLeft = transform.localScale.x < 0;
        switch (type) {
            case EnemyType.Arrow:
                obj.transform.position += Mathf.RoundToInt((float) Modules.randomDefault.NextDouble() - 0.15f) * Vector3.down * 1.2f;
                break;
            case EnemyType.Shooter:
                obj.transform.localScale = Vector3.one;
                obj.transform.position += Modules.randomDefault.Next(-2, 2) * Vector3.left;
                break;
        }
    }
    private void Awake() {
        GameObject.Destroy(GetComponent<SpriteRenderer>());
    }
}

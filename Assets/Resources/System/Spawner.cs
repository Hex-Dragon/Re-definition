using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject prefeb;
    public EnemyType type;
    public enum EnemyType { Arrow, Mover, Heavy }
    public GameObject parent;
    public void Spawn() {
        GameObject obj = Instantiate(prefeb, transform);
        obj.transform.parent = parent.transform;
        obj.GetComponent<EnemyBase>().movingLeft = transform.localScale.x < 0;
        if (type == EnemyType.Arrow) {
            obj.transform.position += Mathf.RoundToInt((float) Modules.randomDefault.NextDouble() - 0.15f) * Vector3.down * 0.8f;
        }
    }
}

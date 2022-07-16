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
    }
}

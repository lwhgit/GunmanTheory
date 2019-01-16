using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSound : MonoBehaviour {
    void Start() {
        Destroy(gameObject, 2);
    }

    public FireSound Spawn(Vector3 position) {
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = position;
        return obj.GetComponent<FireSound>();
    }
}

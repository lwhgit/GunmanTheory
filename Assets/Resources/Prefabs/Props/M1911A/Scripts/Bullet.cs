using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 10);
    }

    public Bullet Spawn(Vector3 position) {
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = position;
        return obj.GetComponent<Bullet>();
    }

    public void SetRotation(Vector3 rotation) {
        gameObject.transform.eulerAngles = rotation;
    }

    public Rigidbody GetRigidbody() {
        return gameObject.GetComponent<Rigidbody>();
    }
}

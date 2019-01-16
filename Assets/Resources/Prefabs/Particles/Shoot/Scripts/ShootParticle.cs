using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootParticle : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 1);
    }

    public void SetRotation(Vector3 rotation) {
        gameObject.transform.eulerAngles = rotation;
    }

    public ShootParticle Spawn(Vector3 position) {
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = position;
        return gameObject.GetComponent<ShootParticle>();
    }
}

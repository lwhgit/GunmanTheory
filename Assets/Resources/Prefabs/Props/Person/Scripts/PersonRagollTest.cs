using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonRagollTest : MonoBehaviour {

    public Rigidbody stomachRigidbody;

    void Start() {
        StartCoroutine(TestCorutine());
    }


    void Update() {

    }

    private IEnumerator TestCorutine() {
        yield return new WaitForSeconds(0.1f);
        stomachRigidbody.AddForce(gameObject.transform.forward * -50 + gameObject.transform.up * 10, ForceMode.VelocityChange);

    }
}

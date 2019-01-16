using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTest : MonoBehaviour {
    public GameObject rightUpperArm;
    public GameObject rightLowerArm;
    public GameObject rightParm;

    public GameObject stomach;
    public GameObject chest;
    public GameObject pelvis;

    private Vector3 rightUpperArmDefaultRotation = Vector3.zero;
    private Vector3 rightLowerArmDefaultRotation = Vector3.zero;
    private Vector3 rightParmDefaultRotation = Vector3.zero;
    private Vector3 stomachDefaultRotation = Vector3.zero;
    private Vector3 chestDefaultRotation = Vector3.zero;
    private Vector3 pelvisDefaultRotation = Vector3.zero;

    void Start() {

        rightUpperArmDefaultRotation = rightUpperArm.transform.localEulerAngles;
        rightLowerArmDefaultRotation = rightLowerArm.transform.localEulerAngles;
        rightParmDefaultRotation = rightParm.transform.localEulerAngles;
        stomachDefaultRotation = stomach.transform.eulerAngles;
        chestDefaultRotation = chest.transform.eulerAngles;
        pelvisDefaultRotation = pelvis.transform.eulerAngles;

        StartCoroutine(UpRightArmCorutine());
        StartCoroutine(LookAtCorutine(90));
    }
    
    void Update() {

    }

    private IEnumerator UpRightArmCorutine() {
        yield return new WaitForSeconds(2);

        float delay = 0;
        float delta = 0;

        Vector3 maxUpperArmRotation = new Vector3(5, 13, 90);
        Vector3 maxLowerArmRotation = new Vector3(13, 16, 3);
        Vector3 maxParmRotation = new Vector3(-8.5f, 3, 7);

        bool loop = true;

        loop = true;
        delta = 0.03f;
        delay = 0.005f;
        for (float i = 0; loop; i += delta) {
            if (i > 1) {
                i = 1;
                loop = false;
            }
            float value = 0.5f * (-Mathf.Cos(i * Mathf.PI) + 1);

            rightUpperArm.transform.localEulerAngles = rightUpperArmDefaultRotation +  Vector3.Lerp(Vector3.zero, maxUpperArmRotation, value);
            rightLowerArm.transform.localEulerAngles = rightLowerArmDefaultRotation + Vector3.Lerp(Vector3.zero, maxLowerArmRotation, value);
            rightParm.transform.localEulerAngles = rightParmDefaultRotation + Vector3.Lerp(Vector3.zero, maxParmRotation, value);

            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator LookAtCorutine(float angle) {
        yield return new WaitForSeconds(2);

        float delay = 0;
        float delta = 0;

        Vector3 maxStomachRotation = new Vector3(0, angle * 0.2f, 0);
        Vector3 maxChestRotation = new Vector3(0, angle * 0.8f, 0);
        Vector3 maxPelvisRotation = new Vector3(0, angle * 0.2f, 0);

        bool loop = true;

        loop = true;
        delta = 0.03f;
        delay = 0.005f;
        for (float i = 0; loop; i += delta) {
            if (i > 1) {
                i = 1;
                loop = false;
            }
            float value = 0.5f * (-Mathf.Cos(i * Mathf.PI) + 1);

            stomach.transform.eulerAngles = stomachDefaultRotation + Vector3.Lerp(Vector3.zero, maxStomachRotation, value);
            chest.transform.eulerAngles = chestDefaultRotation + Vector3.Lerp(Vector3.zero, maxChestRotation, value);
            pelvis.transform.eulerAngles = pelvisDefaultRotation + Vector3.Lerp(Vector3.zero, maxPelvisRotation, value);

            yield return new WaitForSeconds(delay);
        }
    }
}

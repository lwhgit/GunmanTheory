using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

    public static int TURN_DIRECTION_CW = 0;
    public static int TURN_DIRECTION_CCW = 1;

    private int currentPosition = 0;
    
    void Start() {
        StartCoroutine(CameraPositionSetCorutine());
    }

    void Update() {

    }

    private IEnumerator CameraPositionSetCorutine() {
        int[] positions = new int[] { 1, 4, 7, 1, 7, 1, 3, 4 };
        for (int i = 0;i < 3;i ++) {
            for (int j = 0;j < positions.Length; j ++) {
                GotoSeat(positions[j], GameCamera.TURN_DIRECTION_CCW);
                yield return new WaitForSeconds(3);
            }
        }
    }

    private IEnumerator CameraMoveCorutine(int position, int direction) {
        if (position == currentPosition) {
            yield return null;
        } else {
            float tmp = position - currentPosition;
            float step = tmp > 0 ? tmp : 8 + tmp;
            float movePosition = 0;

            if (direction == GameCamera.TURN_DIRECTION_CCW) {
                for (float delta = 0;delta < step;delta += 0.01f * step) {
                    movePosition = currentPosition + delta;
                    SetCameraPosition(movePosition);
                    yield return new WaitForSeconds(0.005f);
                }
            }

            currentPosition = position;

        }
    }

    private void SetCameraPosition(float position) {
        float cameraRot = position * -45;
        float cameraX = Mathf.Sin(position / 4f * Mathf.PI) * 7f;
        float cameraY = 2;
        float cameraZ = -Mathf.Cos(position / 4f * Mathf.PI) * 7f;
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
        gameObject.transform.localEulerAngles = new Vector3(0, cameraRot, 0);
    }

    public void GotoSeat(int position, int direction) {
        StartCoroutine(CameraMoveCorutine(position, direction));
    }
}

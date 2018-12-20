using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Newtonsoft.Json.Linq;

public class RoomCreateSceneManager : MonoBehaviour {

    private SocketEventHandler socketEventHandler = null;

    public InputField roomNameInput;
    public Slider maxPersonnelSlider;
    public Text maxPersonnelView;

    public Button cancelButton;
    public Button createButton;

    void Start() {

        if (SharedArea.isLoggedIn) {
            socketEventHandler = new SocketEventHandler(this);

            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());
            StartCoroutine(SharedArea.socketClient.DataSendCorutine());
        }

    }
    

    void Update() {

    }

    void OnApplicationQuit() {
        SharedArea.socketClient.Disconnect();
    }

    private void RequestCreateRoom() {
        JObject jobj = new JObject();
        jobj.Add("request", "create room");

        JObject config = new JObject();
        config.Add("name", roomNameInput.text);
        config.Add("maxPersonnel", ((int) maxPersonnelSlider.value));
        jobj.Add("config", config);

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }
    
    public void OnMaxPersonnelSliderChanged() {
        int value = (int) maxPersonnelSlider.value;
        maxPersonnelView.text = value.ToString();
        Debug.Log(value);
    }

    public void OnCancelButtonClicked() {
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnCreateButtonClicked() {
        if (roomNameInput.text.Length > 0) {
            RequestCreateRoom();
        } else {
            Debug.Log("Input room name.");
        }
    }

    private class SocketEventHandler : SocketClient.ISocketEventListener {

        private RoomCreateSceneManager roomCreateSceneManager = null;

        public SocketEventHandler(RoomCreateSceneManager manager) {
            roomCreateSceneManager = manager;
        }

        public void OnConnected() {
            Debug.Log("Connected.");
        }

        public void OnConnectionFailed() {
            Debug.Log("Connection failed.");
        }

        public void OnData(byte[] buffer) {

            string msg = Encoding.UTF8.GetString(buffer);

            Debug.Log(msg);

            JObject jobj = JObject.Parse(msg);

            string request = jobj.GetValue("request").ToString();

            if (request.Equals("create room")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    Debug.Log("Room create successed.");
                    SceneManager.LoadScene("RoomScene");
                } else if (result.Equals("failed")) {
                    string message = jobj.GetValue("message").ToString();
                    Debug.Log("Room create failed. " + message);
                }
            }
        }

        public void OnDisconnected() {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Newtonsoft.Json.Linq;

public class LoginSceneManager : MonoBehaviour {

    public InputField nicknameInput;
    public Button loginButotn;
    public Button reconncetionButton;
    public TextView responseView;

    private SocketClient socketClient = null;
    private SocketEventHandler socketEventHandler = null;

	void Start () {
        socketEventHandler = new SocketEventHandler(this);

        socketClient = new SocketClient();
        socketClient.SetSocketEventListener(socketEventHandler);
        ConnectToServer();

        StartCoroutine(socketClient.DataReceiveCorutine());
	}
	
	void Update () {

    }
    
    void OnApplicationQuit() {
        SharedArea.socketClient.Disconnect();
    }

    private void ConnectToServer() {
        socketClient.Connect(Config.SERVER_IP, Config.LOGIN_SERVER_PORT);
    }

    public void OnLoginButtonClicked() {
        if (nicknameInput.text.Length == 0) {
            responseView.SetText("Please input nickname.");
            responseView.ShakeText();
        } else {
            SendLoginNickname(nicknameInput.text);
        }
    }

    public void OnReconncetionButtonClicked() {
        reconncetionButton.gameObject.SetActive(false);
        ConnectToServer();
    }

    private void SendLoginNickname(string nickname) {
        JObject jobj = new JObject();
        jobj.Add("request", "register");
        jobj.Add("nickname", nickname);

        socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private class SocketEventHandler : SocketClient.ISocketEventListener {

        private LoginSceneManager loginSceneManager = null;

        public SocketEventHandler(LoginSceneManager manager) {
            loginSceneManager = manager;
        }

        public void OnConnected() {
            Debug.Log("Connected.");
            loginSceneManager.responseView.SetText("");
            loginSceneManager.loginButotn.interactable = true;
        }

        public void OnConnectionFailed() {
            Debug.Log("Connection failed.");
            loginSceneManager.responseView.SetText("Cannot connect to server.");
            loginSceneManager.responseView.ShakeText();
            loginSceneManager.reconncetionButton.gameObject.SetActive(true);
            loginSceneManager.loginButotn.interactable = false;
        }

        public void OnData(byte[] buffer) {
            string msg = Encoding.UTF8.GetString(buffer);

            Debug.Log(msg);

            JObject jobj = JObject.Parse(msg);

            string request = jobj.GetValue("request").ToString();

            if (request.Equals("register")) {
                Debug.Log("flag1");
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    Debug.Log("flag2");

                    loginSceneManager.responseView.SetText("Successed.");

                    Config.id = (int) jobj.GetValue("id");
                    Config.nickname = jobj.GetValue("nickname").ToString();

                    loginSceneManager.socketClient.Disconnect();

                    SceneManager.LoadScene("LobbyScene");
                } else if (result.Equals("failed")) {
                    string message = jobj.GetValue("message").ToString();
                    loginSceneManager.responseView.SetText(message);
                    loginSceneManager.responseView.ShakeText();
                }
            }
        }

        public void OnDisconnected() {

        }
    }
}

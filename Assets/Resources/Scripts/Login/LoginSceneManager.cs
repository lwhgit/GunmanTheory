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
    public Text responseView;

    private SocketClient socketClient = null;
    private SocketEventHandler socketEventHandler = null;

	void Start () {
        socketEventHandler = new SocketEventHandler(this);

        socketClient = new SocketClient();
        socketClient.SetSocketEventListener(socketEventHandler);
        socketClient.Connect(Config.SERVER_IP, Config.LOGIN_SERVER_PORT);
        StartCoroutine(socketClient.DataReceiveCorutine());
	}
	
	void Update () {
		
	}

    public void OnLoginButtonClick() {
        SendLoginNickname(nicknameInput.text);
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
        }

        public void OnConnectionFailed() {
            Debug.Log("Connection failed.");
        }

        public void OnData(byte[] buffer) {

            string msg = Encoding.UTF8.GetString(buffer);
            JObject jobj = JObject.Parse(msg);

            string request = jobj.GetValue("request").ToString();

            if (request.Equals("register")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    loginSceneManager.responseView.text = "Successed.";

                    Config.id = (int) jobj.GetValue("id");
                    Config.nickname = jobj.GetValue("nickname").ToString();

                    loginSceneManager.socketClient.Disconnect();

                    SceneManager.LoadScene("LobbyScene");
                } else if (result.Equals("failed")) {
                    string message = jobj.GetValue("message").ToString();
                    loginSceneManager.responseView.text = message;
                }
            }
        }

        public void OnDisconnected() {

        }
    }
}

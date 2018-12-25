using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Newtonsoft.Json.Linq;

public class GameSceneManager : MonoBehaviour {

    private SocketEventHandler socketEventHandler = null;

    void Start() {

    }
    

    void Update() {
        if (SharedArea.isLoggedIn) {
            socketEventHandler = new SocketEventHandler(this);

            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());
            StartCoroutine(SharedArea.socketClient.DataSendCorutine());
        }
    }

    private class SocketEventHandler : SocketClient.ISocketEventListener {

        private GameSceneManager gameSceneManager = null;

        public SocketEventHandler(GameSceneManager manager) {
            gameSceneManager = manager;
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

            
        }

        public void OnDisconnected() {

        }
    }
}

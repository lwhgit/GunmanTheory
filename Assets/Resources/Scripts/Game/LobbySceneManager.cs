using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Newtonsoft.Json.Linq;

public class LobbySceneManager : MonoBehaviour {

    public RectTransform roomListViewContent;
    public RoomItem roomItemOrigin;

    public Text responseView;
    
    private SocketEventHandler socketEventHandler = null;

    void Start () {
        Debug.Log("das");

        socketEventHandler = new SocketEventHandler(this);

        if (Config.id != -1 && !SharedArea.isLoggedIn) {
            Debug.Log("ID: " + Config.id + "\t\tNickname: " + Config.nickname);
            SharedArea.socketClient = new SocketClient();
            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            SharedArea.socketClient.Connect(Config.SERVER_IP, Config.GAME_SERVER_PORT);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());

            LoginToGameServer();

            Config.userManager = new UserManager();
        } else if (SharedArea.isLoggedIn) {
            
            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());

            RequestUserList();
            RequestRoomList();
        }
    }
	
	void Update () {
		
	}

    void OnApplicationQuit() {
        SharedArea.socketClient.Disconnect();
    }

    private void RefreshRoomListView(JArray jarr) {                                                     // 수정 좀 해야할것같이 생김

        for (int i = 0;i < roomListViewContent.childCount;i ++) {
            Destroy(roomListViewContent.GetChild(i).gameObject);
        }

        int length = jarr.Count;
        roomListViewContent.sizeDelta = new Vector2(0, 240 * length + 80);

        int number = 0;
        foreach (JObject item in jarr.Children<JObject>()) {
            int roomId = (int) item.GetValue("id");
            string roomName = (string) item.GetValue("config").ToObject<JObject>().GetValue("name");

            RoomItem roomItem = Instantiate(roomItemOrigin.gameObject).GetComponent<RoomItem>();
            roomItem.SetRoomItemEvent(delegate {
                RequestEnterRoom(roomId);
            });
            roomItem.SetRoomId(roomId);
            roomItem.SetRoomName(roomName);
            roomItem.AppendTo(roomListViewContent.gameObject, number);
            number++;

        }

    }


    private void LoginToGameServer() {
        JObject jobj = new JObject();
        jobj.Add("request", "login");
        jobj.Add("id", Config.id);
        jobj.Add("nickname", Config.nickname);

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private void RequestUserList() {
        JObject jobj = new JObject();
        jobj.Add("request", "ask user list");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private void RequestRoomList() {
        JObject jobj = new JObject();
        jobj.Add("request", "ask room list");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private void RequestEnterRoom(int roomId) {
        Debug.Log("Request Enter Room" + roomId);

        JObject jobj = new JObject();
        jobj.Add("request", "enter room");
        jobj.Add("roomId", roomId);

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    public void OnRoomRefreshButtonClicked() {
        RequestRoomList();
    }

    public void OnCreateRoomButtonClicked() {
        SceneManager.LoadScene("RoomCreateScene");
    }

    private class SocketEventHandler : SocketClient.ISocketEventListener {

        private LobbySceneManager lobbySceneManager = null;

        public SocketEventHandler(LobbySceneManager manager) {
            lobbySceneManager = manager;
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

            if (request.Equals("login")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    Debug.Log("Login successed.");
                    SharedArea.isLoggedIn = true;
                    lobbySceneManager.RequestUserList();
                    lobbySceneManager.RequestRoomList();
                } else if (result.Equals("failed")) {
                    string message = jobj.GetValue("message").ToString();
                    Debug.Log("Login failed. " + message);
                }
            } else if (request.Equals("ask user list")) {
                JArray jarr = (JArray) jobj.GetValue("userList");

            } else if (request.Equals("ask room list")) {
                JArray jarr = (JArray) jobj.GetValue("roomList");

                lobbySceneManager.RefreshRoomListView(jarr);
            } else if (request.Equals("enter room")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    SceneManager.LoadScene("RoomScene");
                } else if (result.Equals("failed")) {
                    lobbySceneManager.responseView.text = jobj.GetValue("message").ToString();
                }
            }
        }

        public void OnDisconnected() {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

using Newtonsoft.Json.Linq;

public class LobbySceneManager : MonoBehaviour {

    public RectTransform roomListViewContent;
    public RoomItem roomItemOrigin;
    
    private SocketEventHandler socketEventHandler = null;

    void Start () {
        socketEventHandler = new SocketEventHandler(this);

        if (Config.id != -1 && !SharedArea.isLoggedIn) {
            Debug.Log("ID: " + Config.id + "\t\tNickname: " + Config.nickname);
            SharedArea.socketClient = new SocketClient();
            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            SharedArea.socketClient.Connect(Config.SERVER_IP, Config.GAME_SERVER_PORT);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());

            LoginToGameServer();

            Config.userManager = new UserManager();
        }

        if (SharedArea.isLoggedIn) {
            
            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());

            RequestUserList();
        }
    }
	
	void Update () {
		
	}

    void OnApplicationQuit() {
        SharedArea.socketClient.Disconnect();
    }

    private void RefreshRoomListView(JArray jarr) {                                                     // 수정 좀 해야할것같이 생김
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

    public void OnCreateRoomButtonClicked() {
        SceneManager.LoadScene("RoomCreateScene");
        /*
        JObject jobj = new JObject();
        jobj.Add("request", "create room");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
        */
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
                    lobbySceneManager.RequestUserList();                                        // 여기로 옮기면 오류남
                } else if (result.Equals("failed")) {                                           //
                    string message = jobj.GetValue("message").ToString();                       //
                    Debug.Log("Login failed. " + message);                                      //
                }                                                                               //
            } else if (request.Equals("ask user list")) {                                       //
                JArray jarr = (JArray) jobj.GetValue("userList");                               //
                lobbySceneManager.RequestRoomList();                                            // 이거를

            } else if (request.Equals("ask room list")) {
                JArray jarr = (JArray) jobj.GetValue("roomList");

                lobbySceneManager.RefreshRoomListView(jarr);
            } else if (request.Equals("enter room")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    SceneManager.LoadScene("RoomScene");
                }
            }
        }

        public void OnDisconnected() {

        }
    }
}

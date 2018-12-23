using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Newtonsoft.Json.Linq;

public class RoomSceneManager : MonoBehaviour {

    private SocketEventHandler socketEventHandler = null;

    public MemberItem[] memberItems;

    public Button leaveButton;
    public Button startButton;

    void Start() {
        if (SharedArea.isLoggedIn) {
            socketEventHandler = new SocketEventHandler(this);

            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());
            StartCoroutine(SharedArea.socketClient.DataSendCorutine());

            RequestRoomChiefData();
            RequestRoomMemberList();
        }
    }
    

    void Update() {

    }

    void OnApplicationQuit() {
        SharedArea.socketClient.Disconnect();
    }

    private void HandleChiefPermission(int authId) {                      //함수 이름 마음에 안듦
        if (authId == SharedArea.id) {
            startButton.interactable = true;
        } else {
            startButton.interactable = false;
        }
    }

    private void RefreshMemberListView(JArray jarr) {
        List<JObject> items = jarr.ToObject<List<JObject>>();

        for (int i = 0;i < 8;i ++) {
            MemberItem memberItem = memberItems[i];

            JObject item = (JObject) items[i];
            int authId = (int) item.GetValue("authId");
            string authNickname = (string) item.GetValue("authNickname");

            if (authId >= 0) {
                memberItem.SetNumberView(i + 1);
                memberItem.SetNicknameView(authNickname);
            } else {
                memberItem.SetEmptyState();
            }
        }
    }

    private void RequestRoomChiefData() {
        JObject jobj = new JObject();
        jobj.Add("request", "ask room chief data");
        
        
        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private void RequestRoomMemberList() {
        JObject jobj = new JObject();
        jobj.Add("request", "ask room member list");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    private void RequestRoomLeave() {
        JObject jobj = new JObject();
        jobj.Add("request", "leave room");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    public void OnLeaveButtonClicked() {
        RequestRoomLeave();
    }

    private class SocketEventHandler : SocketClient.ISocketEventListener {

        private RoomSceneManager roomSceneManager = null;

        public SocketEventHandler(RoomSceneManager manager) {
            roomSceneManager = manager;
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

            if (request.Equals("ask room chief data")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    JObject chief = (JObject) jobj.GetValue("chief");
                    int chiefAuthId = (int) chief.GetValue("authId");

                    roomSceneManager.HandleChiefPermission(chiefAuthId);
                }

            } else if (request.Equals("ask room member list")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    JArray jarr = (JArray) jobj.GetValue("memberList");
                    roomSceneManager.RefreshMemberListView(jarr);
                }
            } else if (request.Equals("member entered")) {
                roomSceneManager.RequestRoomMemberList();
            } else if (request.Equals("member left")) {
                roomSceneManager.RequestRoomMemberList();
            } else if (request.Equals("leave room")) {
                SceneManager.LoadScene("LobbyScene");
            } else if (request.Equals("chief changed")) {
                JObject chief = (JObject) jobj.GetValue("chief");
                int chiefAuthId = (int) chief.GetValue("authId");

                roomSceneManager.HandleChiefPermission(chiefAuthId);
            }
        }

        public void OnDisconnected() {

        }
    }
}

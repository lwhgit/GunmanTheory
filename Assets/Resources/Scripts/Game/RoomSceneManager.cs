using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json.Linq;

public class RoomSceneManager : MonoBehaviour {

    private SocketEventHandler socketEventHandler = null;

    public MemberItem[] memberItems;

    void Start() {
        if (SharedArea.isLoggedIn) {
            socketEventHandler = new SocketEventHandler(this);

            SharedArea.socketClient.SetSocketEventListener(socketEventHandler);
            StartCoroutine(SharedArea.socketClient.DataReceiveCorutine());

            RequestRoomMemberList();
        }
    }
    

    void Update() {

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
                
            }
        }
    }

    private void RequestRoomMemberList() {
        JObject jobj = new JObject();
        jobj.Add("request", "ask room member list");

        SharedArea.socketClient.Send(Encoding.UTF8.GetBytes(jobj.ToString()));
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

            if (request.Equals("ask room member list")) {
                string result = jobj.GetValue("result").ToString();

                if (result.Equals("successed")) {
                    JArray jarr = (JArray) jobj.GetValue("memberList");
                    Debug.Log(jarr.ToString());
                    roomSceneManager.RefreshMemberListView(jarr);
                }
            } else if (request.Equals("member entered")) {
                roomSceneManager.RequestRoomMemberList();
            }
        }

        public void OnDisconnected() {

        }
    }
}

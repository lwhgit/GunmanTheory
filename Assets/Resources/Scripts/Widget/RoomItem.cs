using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {

    public delegate void ClickEvent();

    private ClickEvent clickEvent = null;

    public Text roomIdView;
    public Text roomNameView;

    private int roomId = -1;
    private string roomName = null;
    
    
    void Start() {

    }
    
    void Update() {
        
    }

    public void SetRoomItemEvent(ClickEvent e) {
        clickEvent = e;
    }

    public void AppendTo(GameObject obj, int number) {
        gameObject.transform.SetParent(obj.transform, false);
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, - (number * 240 + 80), 0);
    }

    public void SetRoomId(int id) {
        roomId = id;
        roomIdView.text = "No." + id;
    }

    public void SetRoomName(string name) {
        roomName = name;
        roomNameView.text = roomName;
    }

    public void OnClicked() {
        Debug.Log("id" + roomId);
        clickEvent();
    }


}

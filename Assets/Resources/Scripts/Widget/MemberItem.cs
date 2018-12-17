using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberItem : MonoBehaviour {

    public Text numberView;
    public Text nicknameView;

    public void SetNumberView(int n) {
        numberView.text = "No." + n;
    }

    public void SetNicknameView(string nickname) {
        nicknameView.text = nickname;
    }
}

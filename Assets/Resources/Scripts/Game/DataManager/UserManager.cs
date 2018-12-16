using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager {
    private List<User> userList = null;

    public UserManager() {
        userList = new List<User>();
    }

    public class User {
        private int authId = -1;
        private string authNickname = null;

        public User(int authId, string authNickname) {
            this.authId = authId;
            this.authNickname = authNickname;
        }

        public int GetAuthId() {
            return authId;
        }

        public string GetAuthNickname() {
            return authNickname;
        }
    }
}

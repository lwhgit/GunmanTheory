using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedArea {
    public static bool isLoggedIn = false;
    public static SocketClient socketClient = null;

    public static int id = -1;
    public static string nickname = null;
}

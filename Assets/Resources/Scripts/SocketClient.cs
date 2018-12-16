using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketClient {
    private ISocketEventListener eventListener = null;
    private Socket socket = null;

    public SocketClient() {
        
    }

    public void SetSocketEventListener(ISocketEventListener listener) {
        eventListener = listener;
    }

    public void Connect(string ip, int port) {
        try {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Connect(IPAddress.Parse(ip), port);
            if (eventListener != null) {
                eventListener.OnConnected();
            }
        } catch (Exception) {
            if (eventListener != null) {
                eventListener.OnConnectionFailed();
            }
        }
    }

    public void Disconnect() {
        socket.Close();
    }

    public bool isSocketConnected() {
        if (socket == null) {
            return false;
        }
        return socket.Connected;
    }

    public void Send(byte[] buffer) {
        socket.Send(buffer);
    }

    public IEnumerator DataReceiveCorutine() {
        int bufferSize = 0;
        while (true) {
            if (isSocketConnected()) {
                bufferSize = socket.Available;
                if (bufferSize > 0) {
                    byte[] buffer = new byte[bufferSize];
                    socket.Receive(buffer);
                    if (eventListener != null) {
                        eventListener.OnData(buffer);
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public interface ISocketEventListener {
        void OnConnected();
        void OnConnectionFailed();
        void OnDisconnected();
        void OnData(byte[] buffer);
    }
}
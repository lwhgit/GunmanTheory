using System;
using System.IO;
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
    private AsyncCallback dataReceiveHandler = null;

    private Queue<byte[]> queueForSend = null;
    private Queue<byte[]> queueForReceive = null;

    public SocketClient() {
        dataReceiveHandler = new AsyncCallback(DataReceiveHandler);

        queueForSend = new Queue<byte[]>();
        queueForReceive = new Queue<byte[]>();
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
            
            WaitForReceive();
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
        queueForSend.Enqueue(buffer);
    }

    private void WaitForReceive() {
        AsyncObject ao = new AsyncObject(4096);
        socket.BeginReceive(ao.buffer, 0, ao.buffer.Length, SocketFlags.None, dataReceiveHandler, ao);
    }

    private void DataReceiveHandler(IAsyncResult asyncResult) {
        try {
            AsyncObject ao = (AsyncObject) asyncResult.AsyncState;

            int length = socket.EndReceive(asyncResult);

            if (length > 0) {
                byte[] buffer = new byte[length];
                Array.Copy(ao.buffer, buffer, length);

                queueForReceive.Enqueue(buffer);
                
            }

            WaitForReceive();
        } catch (Exception) {

        }
    }

    public IEnumerator DataSendCorutine() {
        while (true) {
            if (queueForSend.Count > 0) {
                byte[] buffer = queueForSend.Dequeue();

                socket.Send(buffer);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator DataReceiveCorutine() {
        while (true) {
            if (queueForReceive.Count > 0) {
                byte[] buffer = queueForReceive.Dequeue();
                
                if (eventListener != null) {
                    eventListener.OnData(buffer);
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

    private class AsyncObject {
        public byte[] buffer = null;

        public AsyncObject(int bufferSize) {
            buffer = new byte[bufferSize];
        }
    }
}
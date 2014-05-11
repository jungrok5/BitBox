using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

namespace BitBoxUnity.Core
{
    public class AppSession : SessionBase<ushort>
    {
        protected static readonly int BUFFER_SIZE = 8192;

        protected Socket m_Socket;
        protected byte[] m_RecvBuffer;

        public override void Connect(string remoteAddress, int port)
        {
            if (IsConnected() == true)
                return;
            
            base.Connect(remoteAddress, port);

            m_RecvBuffer = new byte[BUFFER_SIZE];
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            m_Socket.BeginConnect(remoteAddress, port, ConnectCallback, m_Socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
                m_Socket = socket;

                OnConnected(string.Format("{0}:{1}", RemoteAddress, Port));

                m_Socket.BeginReceive(m_RecvBuffer, 0, BUFFER_SIZE, 0, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                OnError(null, e);
            }
        }

        public override void Disconnect()
        {
            if (IsConnected() == false)
                return;

            OnDisconnected(string.Format("{0}:{1}", RemoteAddress, Port));

            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
            m_Socket = null;

            m_RecvBuffer = null;

            base.Disconnect();
        }

        public override bool IsConnected()
        {
            if (m_Socket == null)
                return false;
            if (m_Socket.Connected == false)
                return false;
            return true;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (IsConnected() == false)
                return;

            int receivedBytes = 0;
            try
            {
                receivedBytes = m_Socket.EndReceive(ar);
                if (receivedBytes <= 0)
                {
                    Disconnect();
                    return;
                }

                // 이 이벤트를 받는쪽에서는 패킷이 잘려서 오는것에 대한 처리를 해야한다
                OnReceived(m_RecvBuffer, 0, receivedBytes);

                m_Socket.BeginReceive(m_RecvBuffer, 0, BUFFER_SIZE, 0, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                OnError(null, e);
            }
        }

        public override void Send(ushort id, byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            try
            {
                m_Socket.BeginSend(buffer, offset, length, 0, SendCallback, null);
            }
            catch (Exception e)
            {
                OnError(null, e);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
             if (IsConnected() == false)
                return;

            int sentBytes = 0;
            try
            {
                // TODO 보낸만큼 가지 않았다면 다시 보내야 할텐데
                sentBytes = m_Socket.EndSend(ar);
            }
            catch (Exception e)
            {
                OnError(null, e);
            }
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}

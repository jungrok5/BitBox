using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;

namespace BitBoxUnity.Core
{
    public class AppSession : SessionBase<ushort>
    {
        private static readonly int BUFFER_SIZE = 8192;

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

                if (Connected != null)
                    Connected();

                m_Socket.BeginReceive(m_RecvBuffer, 0, BUFFER_SIZE, 0, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("AppSession.ConnectCallback - {0}", e.Message));
            }
        }

        public override void Disconnect()
        {
            if (IsConnected() == false)
                return;

            if (Disconnected != null)
                Disconnected();

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

                // TODO 패킷받은 처리. 여기서 id를 꺼내서 넘겨줘야한다
                // 패킷객체 생성해서 핸들러로 패스
                if (Received != null)
                    Received(m_RecvBuffer, 0, receivedBytes);

                m_Socket.BeginReceive(m_RecvBuffer, 0, BUFFER_SIZE, 0, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("AppSession.ReceiveCallback - {0}", e.Message));
            }
        }

        public override void Send(ushort id, byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            base.Send(id, buffer, offset, length);

            // TODO id값을 맨앞단에 사이즈와 함께 넣어야한다
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

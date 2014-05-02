using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Util;
using BitBox.Log;

namespace BitBox.Core
{
    // 참고
    // http://msdn.microsoft.com/ko-kr/library/system.net.sockets.socket.receiveasync(v=vs.110).aspx

    // TODO 나중에 정리하자
    public enum CloseReason : int
    {
        Unknown = 0,
    }

    public class Session : IDisposable
    {
        public long m_ID;
        public Server Server;

        public delegate void CloseHandler(Session session, CloseReason reason);
        public CloseHandler Closed;

        private Socket m_Socket;
        public SocketAsyncEventArgs m_RecvSAEA;
        public SocketAsyncEventArgs m_SendSAEA;

        private ConcurrentQueue<ArraySegment<byte>> m_SendQueue = new ConcurrentQueue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> m_SendingList = new List<ArraySegment<byte>>();

        private AtomicBool flagClosed = new AtomicBool();
        private AtomicBool flagClosing = new AtomicBool();
        private AtomicBool flagSending = new AtomicBool();
        private AtomicBool flagReceiving = new AtomicBool();

        public Session(Server server, Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
        {
            Server = server;
            m_Socket = socket;
            m_RecvSAEA = recvSAEA;
            m_SendSAEA = sendSAEA;
            m_RecvSAEA.UserToken = this;
            m_SendSAEA.UserToken = this;
        }

        public void SetSessionID(long id)
        {
            m_ID = id;
        }

        public void StartReceive()
        {
            if (IsConnected() == false)
            {
                flagReceiving.ForceFalse();
                Disconnect();
                return;
            }

            try
            {
                bool pending = true;
                pending = m_Socket.ReceiveAsync(m_RecvSAEA);

                flagReceiving.ForceTrue();

                // pending값이 false라면 동기적으로 처리가 완료되서 여기서 바로 처리해줘야함
                if (pending == false)
                {
                    ProcessReceive(m_RecvSAEA);
                }
            }
            catch (Exception e)
            {
                Logger.Error("StartReceive", e);
                flagReceiving.ForceFalse();
                Disconnect();
                return;
            }
        }

        public virtual void ProcessReceive(SocketAsyncEventArgs e)
        {
            flagReceiving.SetFalse();

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // TODO
                // 여기에 HandleReceive 같은 호출이 이뤄지면 된다

                // TEST 테스트로 받은 데이터를 그대로 넘기자
                Send(e.Buffer, e.Offset, e.BytesTransferred);
            }
            else
            {
                Disconnect();
                return;
            }

            StartReceive();
        }

        public virtual void Send(byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            //
            // TODO 연결 유무 최대 큐 갯수 기타 등등 검사 확인
            //

            byte[] data = Server.m_PooledBufferManager.Take(length);
            Array.Copy(buffer, offset, data, 0, length);
            m_SendQueue.Enqueue(new ArraySegment<byte>(data, 0, length));

            StartSend();
        }

        public void StartSend()
        {
            if (IsConnected() == false)
                return;

            if (m_SendQueue.Count <= 0)
                return;

            if (flagSending.SetTrue() == false)
                return;

            try
            {
                bool pending = true;

                m_SendingList.Clear();

                ArraySegment<byte> arraySeg;
                while (m_SendQueue.TryDequeue(out arraySeg))
                {
                    m_SendingList.Add(arraySeg);
                }
                m_SendSAEA.BufferList = m_SendingList;
                
                pending = m_Socket.SendAsync(m_SendSAEA);

                // pending값이 false라면 동기적으로 처리가 완료되서 여기서 바로 처리해줘야함
                if (pending == false)
                {
                    ProcessSend(m_SendSAEA);
                }
            }
            catch (Exception e)
            {
                Logger.Error("StartReceive", e);
                flagSending.ForceFalse();
                Disconnect();
                return;
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BufferList != null)
            {
                if (e.BufferList.Count != m_SendingList.Count)
                {
                    Logger.Debug(string.Format("Different send count BufferList.Count:{0} / m_SendingList.Count{1}", e.BufferList.Count, m_SendingList.Count));
                }

                foreach (var buf in m_SendingList)
                {
                    Server.m_PooledBufferManager.Return(buf.Array);
                }
                m_SendingList.Clear();
            }

            flagSending.ForceFalse();

            if (e.SocketError == SocketError.Success)
            {
                // 큐에 남은게 있다면 또 보내라
                StartSend();
            }
            else
            {
                Disconnect();
                return;
            }
        }

        public bool IsConnected()
        {
            if (m_Socket == null)
                return false;
            if (flagClosing.IsTrue())
                return false;
            if (flagClosed.IsTrue())
                return false;

            return true;
        }

        public void Disconnect()
        {
            // 받거나 보내고 있는 중이라면 Closing으로 바꾸고 넘어가자
            if (flagReceiving.IsTrue() || flagSending.IsTrue())
            {
                flagClosing.ForceTrue();
                return;
            }

            if (flagClosed.SetTrue() == true)
            {
                m_SendingList.Clear();

                ArraySegment<byte> arraySeg;
                while (m_SendQueue.TryDequeue(out arraySeg))
                {
                    Server.m_PooledBufferManager.Return(arraySeg.Array);
                }

                if (m_Socket != null)
                {
                    m_Socket.CloseEx();
                    m_Socket = null;
                }

                // 얘들은 윗단에서 이벤트 받은 서버쪽에서 풀에 다시 넣는다
                //m_RecvSAEA = null;
                //m_SendSAEA = null;

                Server = null;

                if (Closed != null)
                    Closed(this, CloseReason.Unknown);
            }
        }

        public virtual void Dispose()
        {
        }
    }
}

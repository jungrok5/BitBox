using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BitBox.Log;
using BitBox.Util;

namespace BitBox.Core
{
    public partial class Server : ServiceBase
    {
        protected virtual long GenerateSessionID()
        {
            return KeyGenerator.Alloc();
        }

        public virtual Session CreateSession(Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
        {
            if (m_ServerConfig.SendTimeOut > 0)
                socket.SendTimeout = m_ServerConfig.SendTimeOut;

            if (m_ServerConfig.ReceiveBufferSize > 0)
                socket.ReceiveBufferSize = m_ServerConfig.ReceiveBufferSize;

            if (m_ServerConfig.SendBufferSize > 0)
                socket.SendBufferSize = m_ServerConfig.SendBufferSize;

            socket.NoDelay = true;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            return new Session(this, socket, recvSAEA, sendSAEA);
        }

        protected virtual void OnSessionAccepted(Listener listener, Socket socket)
        {
            if (Stopped.IsTrue())
                return;

            // TODO 여기서 동접 체크?

            SocketAsyncEventArgs recvSAEA;
            if (!m_RecvSAEAPool.TryPop(out recvSAEA))
            {
                Task.Run(() => { socket.CloseEx(); });
                Logger.Error(string.Format("Max connection number {0} was reached!", m_ServerConfig.MaxConnectionNumber));
                return;
            }

            SocketAsyncEventArgs sendSAEA;
            if (!m_SendSAEAPool.TryPop(out sendSAEA))
            {
                Task.Run(() => { socket.CloseEx(); });
                Logger.Error(string.Format("Max connection number {0} was reached!", m_ServerConfig.MaxConnectionNumber));
                return;
            }

            Session session = CreateSession(socket, recvSAEA, sendSAEA);
            if (session == null)
            {
                recvSAEA.UserToken = null;
                sendSAEA.UserToken = null;
                m_RecvSAEAPool.Push(recvSAEA);
                m_SendSAEAPool.Push(sendSAEA);
                Task.Run(() => { socket.CloseEx(); });
                return;
            }

            session.Connected += OnSessionConnected;
            session.Disconnected += OnSessionDisconnected;
            session.Received += OnSessionReceived;
            session.Error += OnSessionError;

            long sessionID = GenerateSessionID();
            session.SetSessionID(sessionID);
            m_Sessions.TryAdd(sessionID, session);

            // TODO
            session.StartReceive();

            Logger.Debug(string.Format("OnSessionAccepted:{0}", session.m_ID));
        }

        protected virtual void OnSessionDisconnected(Session session, CloseReason reason)
        {
            Logger.Debug(string.Format("OnSessionDisconnected:{0}", session.m_ID));
            if (session != null && m_RecvSAEAPool != null && m_SendSAEAPool != null)
            {
                session.m_RecvSAEA.UserToken = null;
                session.m_SendSAEA.UserToken = null;
                // TODO 버퍼 셋팅도 다시 해줘야 하나?
                // 최초 BufferManager에서 할당받은 위치겠지만 
                m_RecvSAEAPool.Push(session.m_RecvSAEA);
                m_SendSAEAPool.Push(session.m_SendSAEA);
            }
            Session removeSession = null;
            m_Sessions.TryRemove(session.m_ID, out removeSession);
        }

        protected virtual void OnSessionConnected(Session session)
        {
            Logger.Debug(string.Format("OnSessionConnected:{0}", session.m_ID));
        }


        protected virtual void OnSessionReceived(Session session, byte[] buffer, int offset, int length)
        {
            Logger.Debug(string.Format("OnSessionReceived:{0}", session.m_ID));
        }

        protected virtual void OnSessionError(Session session, string message, Exception e)
        {
            Logger.Debug(string.Format("OnSessionError:{0}", session.m_ID));
        }
    }
}

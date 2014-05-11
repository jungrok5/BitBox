using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Util;
using BitBox.Log;

namespace BitBox.Core
{
    public class Listener : IDisposable
    {
        public delegate void AcceptHandler(Listener listener, Socket socket);
        public delegate void ErrorHandler(Listener listener, Exception e);

        public AcceptHandler Accepted;
        public ErrorHandler Error;

        public ServerConfig.ListenerConfig m_Config;

        private Socket m_ListenSocket;

        public Server Server;

        public Listener(ServerConfig.ListenerConfig listenerConfig, Server server)
        {
            m_Config = listenerConfig;
            Server = server;
        }

        public bool Start(ServerConfig serverConfig)
        {
            IPAddress addr;
            IPAddress.TryParse(m_Config.IP, out addr);
            if (addr == null)
                addr = IPAddress.Any;
            m_Config.EndPoint = new IPEndPoint(addr, m_Config.Port);

            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                SocketAsyncEventArgs socketEventArg;

                var socketArgsList = new List<SocketAsyncEventArgs>(serverConfig.MaxAcceptOps);

                for (int i = 0; i < serverConfig.MaxAcceptOps; i++)
                {
                    // Accept하는 SocketAsyncEventArgs는 버퍼가 없어야 한다
                    socketEventArg = new SocketAsyncEventArgs();
                    socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(CompletedAccept);
                    socketArgsList.Add(socketEventArg);
                }
                Server.m_AcceptSAEAPool = new ConcurrentStack<SocketAsyncEventArgs>(socketArgsList);

                m_ListenSocket.Bind(m_Config.EndPoint);
                m_ListenSocket.Listen(m_Config.BackLog);

                m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                return StartAccept();
            }
            catch (Exception e)
            {
                OnError(e);
                return false;
            }
        }

        bool StartAccept()
        {
            SocketAsyncEventArgs acceptSAEA;

            if (!Server.m_AcceptSAEAPool.TryPop(out acceptSAEA))
            {
                Logger.Error(string.Format("Max Accept number {0} was reached!", Server.m_ServerConfig.MaxConnectionNumber));
                return false;
            }

            try
            {
                bool pending = m_ListenSocket.AcceptAsync(acceptSAEA);

                // pending값이 false라면 동기적으로 처리가 완료되서 여기서 바로 처리해줘야함
                if (pending == false)
                {
                    ProcessAccept(acceptSAEA);
                }
            }
            catch (Exception e)
            {
                Logger.Error("StartAccept", e);
                // TODO 풀로 돌려보내고 다시 받아라? 하면되나
                Server.m_AcceptSAEAPool.Push(acceptSAEA);
                StartAccept();
            }

            return true;
        }

        void CompletedAccept(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket socket = null;

            if (e.SocketError != SocketError.Success)
            {  
                if (SocketEx.IsIgnorableError(e.SocketError) == false)
                    Logger.Error(string.Format("ProcessAccept:{0}", e.SocketError));

                Task.Run(() => { e.AcceptSocket.CloseEx(); });

                e.AcceptSocket = null;
                Server.m_AcceptSAEAPool.Push(e);
                StartAccept();
                return;
            }

            socket = e.AcceptSocket;

            StartAccept();

            e.AcceptSocket = null;
            Server.m_AcceptSAEAPool.Push(e);

            if (socket != null)
                OnAccepted(socket);
        }

        void OnAccepted(Socket socket)
        {
            if (Accepted != null)
                Accepted(this, socket);
        }

        public void Stop()
        {
            if (m_ListenSocket == null)
                return;

            lock (this)
            {
                if (m_ListenSocket == null)
                    return;

                try
                {
                    m_ListenSocket.Close();
                }
                finally
                {
                    m_ListenSocket = null;
                }

                Server = null;
            }
        }

        void OnError(Exception e)
        {
            if (Error != null)
                Error(this, e);
        }

        void OnError(string errorMessage)
        {
            OnError(new Exception(errorMessage));
        }

        public void Dispose()
        {
            Server = null;
        }
    }
}

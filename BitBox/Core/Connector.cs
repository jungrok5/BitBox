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
    public class Connector : IDisposable
    {
        public delegate void ConnectHandler(Connector connector, Socket socket);
        public delegate void ErrorHandler(Connector connector, Exception e);

        public ConnectHandler Connected;
        public ErrorHandler Error;

        public Server Server;

        public Connector(Server server)
        {
            Server = server;
        }

        public void StartConnect(string remoteAddress, int port, int retry)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(remoteAddress, port, CompletedConnect, socket);
        }

        void CompletedConnect(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
                OnConnected(socket);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        void OnConnected(Socket socket)
        {
            if (Connected != null)
                Connected(this, socket);
        }

        public void Stop()
        {
            Server = null;
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

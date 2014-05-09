using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BitBox.Log;
using BitBox.Util;

namespace BitBox.Core
{
    public enum ServerExecuteType
    {
        Console,
        Service,
    }

    public partial class Server : ServiceBase
    {
        public string Name { get; protected set; }
        public string Version { get; protected set; }

        public bool IsRunning { get; set; }
        public bool IsStopped { get; set; }

        public ServerConfig m_ServerConfig;
        public List<Listener> m_Listeners;

        private ConcurrentDictionary<long, Session> m_Sessions;

        public BufferManager m_BufferManager;
        public PooledBufferManager m_PooledBufferManager;

        public ConcurrentStack<SocketAsyncEventArgs> m_AcceptSAEAPool;
        public ConcurrentStack<SocketAsyncEventArgs> m_RecvSAEAPool;
        public ConcurrentStack<SocketAsyncEventArgs> m_SendSAEAPool;

        public static string ModuleName;
        protected ServerExecuteType ExecuteType;

        public virtual bool Init(ServerExecuteType executeType, string version, string name = null)
        {
            IsRunning = false;
            IsStopped = false;

            ExecuteType = executeType;
            Name = string.IsNullOrEmpty(name) == true ? Process.GetCurrentProcess().ProcessName : name;
            Version = version;
            ModuleName = Name;

            CreateLogger();

            Logger.Info("Init");

            if (LoadServerConfig() == false)
                return false;

            TheadPoolEx.SetMinMaxThreads(
                m_ServerConfig.MinWorkerThreads == 0 ? Environment.ProcessorCount : m_ServerConfig.MinWorkerThreads,
                m_ServerConfig.MaxWorkerThreads == 0 ? Environment.ProcessorCount * 2: m_ServerConfig.MaxWorkerThreads,
                m_ServerConfig.MinCompletionPortThreads == 0 ? Environment.ProcessorCount : m_ServerConfig.MinCompletionPortThreads,
                m_ServerConfig.MaxCompletionPortThreads == 0 ? Environment.ProcessorCount * 2 : m_ServerConfig.MaxCompletionPortThreads);
       
            m_Sessions = new ConcurrentDictionary<long, Session>();
            m_Listeners = new List<Listener>();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                int bufferSize = m_ServerConfig.ReceiveBufferSize;

                if (bufferSize <= 0)
                    bufferSize = 1024 * 4;

                // Send, Recv
                m_BufferManager = new BufferManager(bufferSize * m_ServerConfig.MaxConnectionNumber * 2, bufferSize);

                try
                {
                    m_BufferManager.InitBuffer();

                    int[] poolSizes = new int[] { 4096, 16, 128, 256, 1024 };
                    m_PooledBufferManager = new PooledBufferManager(poolSizes);

                    {
                        SocketAsyncEventArgs socketEventArg;
                        var socketArgsList = new List<SocketAsyncEventArgs>(m_ServerConfig.MaxConnectionNumber);

                        for (int i = 0; i < m_ServerConfig.MaxConnectionNumber; i++)
                        {
                            socketEventArg = new SocketAsyncEventArgs();
                            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(CompletedReceive);
                            m_BufferManager.SetBuffer(socketEventArg);
                            socketArgsList.Add(socketEventArg);
                        }
                        m_RecvSAEAPool = new ConcurrentStack<SocketAsyncEventArgs>(socketArgsList);
                    }

                    {
                        SocketAsyncEventArgs socketEventArg;
                        var socketArgsList = new List<SocketAsyncEventArgs>(m_ServerConfig.MaxConnectionNumber);
                        for (int i = 0; i < m_ServerConfig.MaxConnectionNumber; i++)
                        {
                            socketEventArg = new SocketAsyncEventArgs();
                            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(CompletedSend);
                            //m_BufferManager.SetBuffer(socketEventArg);
                            // Send할때 별도의 풀을 사용할거라서
                            socketEventArg.SetBuffer(null, 0, 0);
                            socketArgsList.Add(socketEventArg);
                        }
                        m_SendSAEAPool = new ConcurrentStack<SocketAsyncEventArgs>(socketArgsList);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("메모리 부족! 최대 접속 허용 인원 설정을 낮추세요", e);
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error("Server.Init", e);
                return false;
            }
        }

        public virtual bool ServerStart()
        {
            Logger.Info("ServerStart");

            for (var i = 0; i < m_ServerConfig.Listeners.Length; i++)
            {
                var listener = new Listener(m_ServerConfig.Listeners[i], this);
                listener.Error += new Listener.ErrorHandler(OnListenerError);
                listener.Accepted += new Listener.AcceptHandler(OnSessionAccepted);

                if (listener.Start(m_ServerConfig))
                {
                    m_Listeners.Add(listener);

                    Logger.Info(string.Format("Listener ({0}) was started", listener.m_Config.EndPoint));
                }
                else
                {
                    Logger.Info(string.Format("Listener ({0}) failed to start", listener.m_Config.EndPoint));

                    for (var j = 0; j < m_Listeners.Count; j++)
                    {
                        m_Listeners[j].Stop();
                    }

                    m_Listeners.Clear();
                    return false;
                }
            }

            IsRunning = true;
            return true;
        }

        public virtual void ServerStop()
        {
            Logger.Info("ServerStop");

            if (IsStopped)
                return;

            lock (this)
            {
                if (IsStopped)
                    return;

                IsStopped = true;

                for (var i = 0; i < m_Listeners.Count; i++)
                {
                    var listener = m_Listeners[i];

                    listener.Stop();
                }

                m_Listeners.Clear();

                SocketAsyncEventArgs eventArgs;
                while (m_RecvSAEAPool.Count > 0)
                {
                    if (m_RecvSAEAPool.TryPop(out eventArgs))
                        eventArgs.Dispose();
                }
                while (m_SendSAEAPool.Count > 0)
                {
                    if (m_SendSAEAPool.TryPop(out eventArgs))
                        eventArgs.Dispose();
                }
                while (m_AcceptSAEAPool.Count > 0)
                {
                    if (m_AcceptSAEAPool.TryPop(out eventArgs))
                        eventArgs.Dispose();
                }
                m_RecvSAEAPool = null;
                m_SendSAEAPool = null;
                m_AcceptSAEAPool = null;
                m_BufferManager = null;
                IsRunning = false;

                m_MainThreadStopEvent.Set();
                m_MainThreadStopEvent.Close();
                m_MainThreadStopEvent.Dispose();
                m_MainThreadStopEvent = null;

                Thread.Sleep(100);

                m_MainThread = null;
            }
        }

        void OnListenerError(Listener listener, Exception e)
        {
            Logger.Error(string.Format("Listener ({0}) error: {1}", listener.m_Config.EndPoint, e.Message), e);
        }

        void CompletedReceive(object sender, SocketAsyncEventArgs e)
        {
            var session = e.UserToken as Session;
            if (session == null)
                return;
            if (e.LastOperation != SocketAsyncOperation.Receive)
                throw new ArgumentException(string.Format("Invalid LastOperation:{0}", e.LastOperation));

            Logger.Debug(string.Format("CompletedReceive:{0}", session.m_ID));

            session.ProcessReceive(e);
        }

        void CompletedSend(object sender, SocketAsyncEventArgs e)
        {
            var session = e.UserToken as Session;
            if (session == null)
                return;
            if (e.LastOperation != SocketAsyncOperation.Send)
                throw new ArgumentException(string.Format("Invalid LastOperation:{0}", e.LastOperation));

            Logger.Debug(string.Format("CompletedSend:{0}", session.m_ID));

            session.ProcessSend(e);
        }

        public virtual bool LoadServerConfig()
        {
            try
            {
                using (StreamReader sr = File.OpenText(System.IO.Directory.GetCurrentDirectory() + @"\Config\Server.config"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ServerConfig));
                    m_ServerConfig = serializer.Deserialize(sr) as ServerConfig;
                    return true;
                }
            }
            catch(Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public virtual bool CreateLogger()
        {
            List<LoggerBase> loggers = new List<LoggerBase>();
            LoggerBase mainLogger = null;
            if (ExecuteType == ServerExecuteType.Service)
            {
                LoggerBase logger = new WindowEventLogger { ApplicationName = string.Format("{0} - {1}", ModuleName, Version) };
                loggers.Add(logger);
            }
            else
            {
                LoggerBase logger = new ConsoleLogger();
                loggers.Add(logger);
                mainLogger = logger;
            }
            Logger.Init(mainLogger, loggers);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsRunning)
                    Stop();
            }

            base.Dispose(disposing);
        }
    }
}

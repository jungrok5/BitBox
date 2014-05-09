﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;
using BitBox.Log;

namespace BitBoxAppServerExample.Scripts
{
    public class ExampleAppServer : Server
    {
        private ConcurrentDictionary<long, ExampleSession> m_Clients = new ConcurrentDictionary<long, ExampleSession>();

        public override bool Init(ServerExecuteType executeType, string version, string name = null)
        {
            if (base.Init(executeType, version, name) == false)
                return false;

            return true;
        }

        public override bool CreateLogger()
        {
            List<LoggerBase> loggers = new List<LoggerBase>();
            LoggerBase mainLogger = new ConsoleLogger();
            loggers.Add(mainLogger);
            loggers.Add(new FileLogger());
            Logger.Init(mainLogger, loggers);

            return true;
        }

        public override Session CreateSession(Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
        {
            return new ExampleSession(this, socket, recvSAEA, sendSAEA);
        }
    }
}
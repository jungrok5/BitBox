using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;
using BitBox.Log;
using BitBox.Test;

namespace BitBox
{
    public class TestServer : Server
    {
        public override bool Init(ServerExecuteType executeType)
        {
            if (base.Init(executeType) == false)
                return false;

            TestModule.Start();
            return true;
        }

        public override void CreateLogger()
        {
            List<LoggerBase> loggers = new List<LoggerBase>();
            LoggerBase mainLogger = new ConsoleLogger();
            loggers.Add(mainLogger);
            loggers.Add(new FileLogger());
            Logger.Init(mainLogger, loggers);
        }

        public override Session CreateSession(Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
        {
            return base.CreateSession(socket, recvSAEA, sendSAEA);
        }
    }
}

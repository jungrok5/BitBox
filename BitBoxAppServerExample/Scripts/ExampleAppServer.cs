using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;
using BitBox.Log;
using BitBoxExample.CSCommon;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace BitBoxAppServerExample.Scripts
{
    public partial class ExampleAppServer : Server
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
            loggers.Add(new FileLogger(System.IO.Directory.GetCurrentDirectory() + @"\Log\ExampleAppServer.log"));
            Logger.Init(mainLogger, loggers);

            return true;
        }

        public override Session CreateSession(Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
        {
            return new ExampleSession(this, socket, recvSAEA, sendSAEA);
        }

        protected override void OnSessionReceived(Session session, byte[] buffer, int offset, int length)
        {
            base.OnSessionReceived(session, buffer, offset, length);

            Packet recvPacket = new Packet(buffer, length);
            Protocol_Handler(session, recvPacket);
        }

        void Protocol_Handler(Session session, Packet packet)
        {
            switch (packet.GetID())
            {
                case ProtocolID.CS_ECHO_APP_REQ:{ On_CS_ECHO_APP_REQ(session, packet); } break;
                default:
                    {
                        Logger.Warning(string.Format("Receive unknown packet id:{0}", packet.GetID()));
                        // 끊어버려?
                    }
                    break;
            }
            if (packet.GetRemainDataSize() > 0)
            {
                Logger.Warning(string.Format("Remain packet data [{0}] {1} bytes", ((ProtocolID)packet.GetID()).ToString(), packet.GetRemainDataSize()));
            }
        }

        public override bool LoadServerConfig()
        {
            try
            {
                using (StreamReader sr = File.OpenText(System.IO.Directory.GetCurrentDirectory() + @"\Config\ExampleAppServer.config"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ServerConfig));
                    m_ServerConfig = serializer.Deserialize(sr) as ServerConfig;
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitBox.Core
{
    public class ServerConfig
    {
        public int SendTimeOut;
        public int SendBufferSize;
        public int ReceiveBufferSize;
        public int MaxConnectionNumber;
        public int MaxAcceptOps;

        public int MinWorkerThreads;
        public int MaxWorkerThreads;
        public int MinCompletionPortThreads;
        public int MaxCompletionPortThreads;

        public class ListenerConfig
        {
            public string IP;
            public int Port;
            public int BackLog;
            [XmlIgnore]
            public IPEndPoint EndPoint;
        }

        [XmlArrayItemAttribute("Listener", IsNullable = false)]
        public ListenerConfig[] Listeners;
    }
}

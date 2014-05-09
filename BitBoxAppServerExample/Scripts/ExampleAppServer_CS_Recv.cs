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

namespace BitBoxAppServerExample.Scripts
{
    public partial class ExampleAppServer : Server
    {
        void On_CS_ECHO_APP_REQ(Session session, Packet packet)
        {
            string data = packet.ReadString();
            long tick = packet.ReadLong();

            Logger.Debug(string.Format("data:{0} tick:{1}", data, tick));
            Send_CS_ECHO_APP_ACK(session, data, tick);
        }
    }
}
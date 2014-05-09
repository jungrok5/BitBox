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
        void SendToClient(Session session, Packet packet)
        {
            if (session == null)
                return;

            session.Send(packet.GetData(), 0, packet.GetTotalPacketSize());
        }
        public void Send_CS_ECHO_APP_ACK(Session session, string data, long tick)
        {
            Packet sendPacket = new Packet(ProtocolID.CS_ECHO_APP_ACK);
            sendPacket.WriteString(data);
            sendPacket.WriteLong(tick);
            SendToClient(session, sendPacket);
        }
    }
}
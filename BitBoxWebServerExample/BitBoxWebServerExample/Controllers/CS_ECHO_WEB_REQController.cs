using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitBox.Web;
using BitBoxExample.CSCommon;
using BitBoxWebServerExample.Scripts.Core;

namespace BitBoxWebServerExample.Controllers
{
    public class CS_ECHO_WEB_REQController : Controller
    {
        public BinaryResult ProcessReceive(Packet recvPacket)
        {
            string data = recvPacket.ReadString();
            long tick = recvPacket.ReadLong();

            Packet sendPacket = new Packet(ProtocolID.CS_ECHO_WEB_ACK);
            sendPacket.WriteString(data);
            sendPacket.WriteLong(tick);

            return new PacketResult(sendPacket);
        }
    }
}

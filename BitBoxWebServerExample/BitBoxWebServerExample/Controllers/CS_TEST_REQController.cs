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
    public class CS_TEST_REQController : Controller
    {
        [HttpPost]
        public BinaryResult ProcessReceive(Packet recvPacket)
        {
            string data = recvPacket.ReadString();

            Packet sendPacket = new Packet((ushort)ProtocolID.CS_TEST_ACK);
            sendPacket.WriteString(data); // echo

            return new PacketResult(sendPacket);
        }

    }
}

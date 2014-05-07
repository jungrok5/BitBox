using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitBox.Web;
using BitBox.Core;
using BitBoxExample.CSCommon;
using BitBoxWebServerExample.Scripts.Core;

namespace BitBoxWebServerExample.Controllers
{
    public class TestController : Controller
    {
        public BinaryResult TestBinary(Packet recvPacket)
        {
            string d1 = recvPacket.ReadString();
            int d2 = recvPacket.ReadInt();

            Packet sendPacket = new Packet(1);
            sendPacket.WriteString("응답데이터");
            sendPacket.WriteInt(11111);

            return new PacketResult(sendPacket);
        }
    }
}

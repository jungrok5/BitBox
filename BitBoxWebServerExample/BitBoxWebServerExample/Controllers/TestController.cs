using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitBox.Web;
using BitBox.Core;

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

            return new BinaryResult() { Data = new ArraySegment<byte>(sendPacket.m_pData, 0, sendPacket.GetTotalPacketSize()), Headers = HttpContext.Response.Headers };
        }
    }
}

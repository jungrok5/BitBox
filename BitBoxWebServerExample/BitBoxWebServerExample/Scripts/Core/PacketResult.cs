using BitBox.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitBoxExample.CSCommon;

namespace BitBoxWebServerExample.Scripts.Core
{
    public class PacketResult : BinaryResult
    {
        public PacketResult(Packet packet)
        {
            Data = new ArraySegment<byte>(packet.GetData(), 0, packet.GetTotalPacketSize());
        }
    }
}
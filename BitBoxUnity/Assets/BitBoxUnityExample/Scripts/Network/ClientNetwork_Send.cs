using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    public void Send_CS_TEST_REQ()
    {
        Packet packet = new Packet((ushort)ProtocolID.CS_TEST_REQ);
        SendPacket_Web(packet);
    }
}
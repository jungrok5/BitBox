using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;
using System;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    public void Send_CS_ECHO_APP_REQ(string data)
    {
        Packet packet = new Packet(ProtocolID.CS_ECHO_APP_REQ);
        packet.WriteString(data);

        long tick = DateTime.Now.Ticks;
        packet.WriteLong(tick);

        Debug.Log(string.Format("SEND to Server:{0}[{1}]", data, tick));

        SendPacket_App(packet);
    }
}
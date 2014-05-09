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

    public void Send_CS_ECHO_APP_REQ_TWICE(string data)
    {
        if (m_AppSession == null)
            return;

        long tick = DateTime.Now.Ticks;

        Packet packetOne = new Packet(ProtocolID.CS_ECHO_APP_REQ);
        packetOne.WriteString(data);       
        packetOne.WriteLong(tick);

        Packet packetTwo = new Packet(ProtocolID.CS_ECHO_APP_REQ);
        packetTwo.WriteString(data);
        packetTwo.WriteLong(tick);

        Debug.Log(string.Format("SEND to Server:{0}[{1}]", data, tick));

        byte[] buffer = new byte[packetOne.GetTotalPacketSize() + packetTwo.GetTotalPacketSize()];
        Array.Copy(packetOne.GetData(), 0, buffer, 0, packetOne.GetTotalPacketSize());
        Array.Copy(packetTwo.GetData(), 0, buffer, packetOne.GetTotalPacketSize(), packetTwo.GetTotalPacketSize());

        m_AppSession.Send(0, buffer, 0, buffer.Length);
    }
}


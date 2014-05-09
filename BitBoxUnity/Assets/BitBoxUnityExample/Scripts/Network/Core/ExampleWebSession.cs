using UnityEngine;
using System.Collections;
using BitBoxUnity.Core;
using BitBoxExample.CSCommon;

public class ExampleWebSession : WebSession
{
    public void Send(Packet packet)
    {
        Send(((ProtocolID)packet.GetID()).ToString(), packet.m_pData, 0, packet.GetTotalPacketSize());
    }
}
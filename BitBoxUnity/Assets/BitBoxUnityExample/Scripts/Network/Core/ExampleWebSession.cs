using UnityEngine;
using System.Collections;
using BitBoxUnity.Core;
using BitBoxExample.CSCommon;

public class ExampleWebSession : WebSession
{
    public void Send(Packet packet)
    {
        Send(packet.GetID().ToString(), packet.GetData(), 0, packet.GetTotalPacketSize());
    }
}
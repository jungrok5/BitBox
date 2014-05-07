using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    void On_CS_TEST_ACK(Packet packet)
    {
        string data = packet.ReadString();
        Debug.Log("RECV from Server:" + data);
    }
}
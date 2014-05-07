using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    void On_CS_ECHO_WEB_ACK(Packet packet)
    {
        string data = packet.ReadString();
        Debug.Log(string.Format("RECV from Server:{0}", data));

        // echo
        ClientNetwork.Instance.Send_CS_ECHO_WEB_REQ(data);
    }
}
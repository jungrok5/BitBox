using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    // TODO 일단 서버가 그대로 에코중이라서 _ACK가 아니라 _REQ다
    void On_CS_ECHO_APP_REQ(Packet packet)
    {
        string data = packet.ReadString();
        long tick = packet.ReadLong();

        Debug.Log(string.Format("RECV from Server:{0}[{1}]", data, tick));

        // echo
        ClientNetwork.Instance.Send_CS_ECHO_APP_REQ(data);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;
using System;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    private static readonly string STRING_FORMAT_RECV = "On_{0}";

    private AppSession m_AppSession;
    private WebSession m_WebSession;

    private Queue<Packet> m_RecvPacketList;

    void Awake()
    {
        m_RecvPacketList = new Queue<Packet>();

        CreateSession_Web();
        CreateSession_App();

        ConnectSession_Web("localhost", 57778);
        //ConnectSession_App("localhost", 12345);
    }

    void ReceiveHandler(Packet packet)
    {
        string handleFuncName = string.Format(STRING_FORMAT_RECV, ((ProtocolID)packet.GetID()).ToString());
        MethodInfo mi = this.GetType().GetMethod(handleFuncName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi == null)
        {
            Debug.LogWarning(string.Format("Not exists message handler function:{0}", handleFuncName));
            return;
        }
        mi.Invoke(this, new object[] { packet });   
    }

    void Update()
    {
        lock (m_RecvPacketList)
        {
            if (m_RecvPacketList.Count <= 0)
                return;

            ReceiveHandler(m_RecvPacketList.Dequeue());         
        }
    }

    void OnConnect(string endpoint)
    {
        Debug.Log(string.Format("OnConnect:{0}", endpoint));
    }

    void OnDisconnect(string endpoint)
    {
        Debug.Log(string.Format("OnDisconnect:{0}", endpoint));
    }

    void OnError(string message, Exception e)
    {
        Debug.Log(string.Format("OnError:{0}", e != null ? e.Message : message));
    }
}

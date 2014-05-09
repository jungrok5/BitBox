using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;
using System;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    public void CreateSession_Web()
    {
        GameObject go = new GameObject("WebSession");
        m_WebSession = go.AddComponent<ExampleWebSession>();

        m_WebSession.Connected += OnConnect;
        m_WebSession.Disconnected += OnDisconnect;
        m_WebSession.Received += OnReceive_Web;
        m_WebSession.Error += OnError;
    }

    public void ConnectSession_Web(string remoteAddress, int port)
    {
        if (m_WebSession == null)
            return;
        m_WebSession.Connect(remoteAddress, port);
    }

    void SendPacket_Web(Packet packet)
    {
        if (m_WebSession == null)
            return;

        m_WebSession.Send(packet);
    }

    void OnReceive_Web(byte[] buffer, int offset, int length)
    {
        Debug.Log(string.Format("OnReceive_Web:{0}", length));

        Packet recvPacket = new Packet(buffer, length);
        lock (m_RecvPacketList)
        {
            m_RecvPacketList.Enqueue(recvPacket);
        }
    }
}
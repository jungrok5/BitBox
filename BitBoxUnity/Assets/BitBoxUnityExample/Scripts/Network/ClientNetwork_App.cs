using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;
using System;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    public void CreateSession_App()
    {
        GameObject go = new GameObject("AppSession");
        m_AppSession = go.AddComponent<AppSession>();

        m_AppSession.Connected += OnConnect;
        m_AppSession.Disconnected += OnDisconnect;
        m_AppSession.Received += OnReceive_App;
        m_AppSession.Error += OnError;
    }

    public void ConnectSession_App(string remoteAddress, int port)
    {
        if (m_AppSession == null)
            return;
        m_AppSession.Connect(remoteAddress, port);
    }

    void SendPacket_App(Packet packet)
    {
        if (m_AppSession == null)
            return;

        m_AppSession.Send(packet.GetID(), packet.m_pData, 0, (ushort)packet.GetTotalPacketSize());
    }

    void OnReceive_App(byte[] buffer, int offset, int length)
    {
        Debug.Log(string.Format("OnReceive_Web:{0}", length));

        Packet recvPacket = new Packet(buffer, length);
        lock (m_RecvPacketList)
        {
            m_RecvPacketList.Enqueue(recvPacket);
        }
    }
}
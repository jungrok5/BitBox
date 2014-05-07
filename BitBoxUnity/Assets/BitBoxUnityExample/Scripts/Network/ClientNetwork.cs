using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BitBoxUnity.Core;
using System.Reflection;
using BitBoxExample.CSCommon;

public partial class ClientNetwork : Singleton<ClientNetwork>
{
    private static readonly string STRING_FORMAT_RECV = "On_{0}";

    private WebSession m_WebSession;
    private Queue<Packet> m_RecvPacketList;

    void Start()
    {
        m_RecvPacketList = new Queue<Packet>();

        GameObject go = new GameObject("WebSession");
        m_WebSession = go.AddComponent<WebSession>();

        m_WebSession.Received += HandleReceive_Web;
        m_WebSession.Connect("localhost", 57778);
    }

    void HandleReceive(Packet packet)
    {
        string handleFuncName = string.Format(STRING_FORMAT_RECV, ((ProtocolID)packet.GetID()).ToString());
        MethodInfo mi = this.GetType().GetMethod(handleFuncName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi == null)
        {
            Debug.LogWarning("Not exists message function:" + handleFuncName);
            return;
        }
        mi.Invoke(this, new object[] { packet });   
    }

    void HandleReceive_Web(byte[] buffer, int offset, int length)
    {
        Packet recvPacket = new Packet(buffer, length);
        lock (m_RecvPacketList)
        {
            m_RecvPacketList.Enqueue(recvPacket);
        }
    }

    void Update()
    {
        lock (m_RecvPacketList)
        {
            if (m_RecvPacketList.Count <= 0)
                return;

            HandleReceive(m_RecvPacketList.Dequeue());         
        }
    }

    void SendPacket_Web(Packet packet)
    {
        if (m_WebSession == null)
            return;

        m_WebSession.Send(((ProtocolID)packet.GetID()).ToString(), packet.m_pData, 0, (ushort)packet.GetTotalPacketSize());
    }
}

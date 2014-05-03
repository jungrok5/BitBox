using UnityEngine;
using System.Collections;
using System.Text;
using System;
using BitBoxUnity.Core;

public class Test : MonoBehaviour 
{
    WebSession session;

    void Start()
    {
        GameObject go = new GameObject("WebSession");
        session = go.AddComponent<WebSession>();
        
        session.Received += HandleReceive;
        session.Connect("localhost", 57778);


        Packet sendPacket = new Packet(1);
        sendPacket.WriteString("보내는데이터");
        sendPacket.WriteInt(22222);
        session.Send("Test/TestBinary", sendPacket.m_pData, 0, sendPacket.GetTotalPacketSize());
    }

    void HandleReceive(byte[] buffer, int offset, int length)
    {
        Packet recvPacket = new Packet(buffer, length);

        Debug.Log(recvPacket.ReadString());
        Debug.Log(recvPacket.ReadInt());
    }
}

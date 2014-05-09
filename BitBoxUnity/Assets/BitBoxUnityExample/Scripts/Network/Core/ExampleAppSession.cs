using UnityEngine;
using System.Collections;
using BitBoxUnity.Core;
using BitBoxExample.CSCommon;
using System;

public class ExampleAppSession : AppSession
{
    private byte[] m_Buffer = new byte[BUFFER_SIZE];
    private int m_BufferPos = 0;

    public void Send(Packet packet)
    {
        Send(packet.GetID(), packet.m_pData, 0, packet.GetTotalPacketSize());
    }

    protected override void OnReceived(byte[] buffer, int offset, int length)
    {
        if (length >= BUFFER_SIZE)
        {
            OnError("Invalid receive data", null);
            return;
        }

        Debug.Log("m_BufferPos:" + m_BufferPos + " length:" + length);

        Array.Copy(buffer, offset, m_Buffer, m_BufferPos, length);
        m_BufferPos += length;

        // 헤더 이상은 왔다
        if (m_BufferPos >= 4)
        {
            ushort packetSize = BitConverter.ToUInt16(m_Buffer, 2);
            Debug.Log("packetSize:" + packetSize);
            if (packetSize < 0 || packetSize >= BUFFER_SIZE - 4)
            {
                OnError("Invalid receive data", null);
                return;
            }

            // 패킷사이즈 보다 더 많이 왔군. 자르자
            if (packetSize + 4 <= m_BufferPos)
            {
                base.OnReceived(m_Buffer, 0, packetSize + 4);
                m_Buffer = new byte[BUFFER_SIZE];
                Array.Copy(m_Buffer, packetSize + 4, m_Buffer, 0, m_BufferPos - packetSize + 4);
                m_BufferPos -= packetSize + 4;
            }
        }
    }
}
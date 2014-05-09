using UnityEngine;
using System.Collections;
using BitBoxUnity.Core;
using BitBoxExample.CSCommon;
using System;

public class ExampleAppSession : AppSession
{
    private byte[] m_Buffer = new byte[Packet.BUFFER_SIZE];
    private int m_BufferPos = 0;

    public void Send(Packet packet)
    {
        Send((ushort)packet.GetID(), packet.GetData(), 0, packet.GetTotalPacketSize());
    }

    protected override void OnReceived(byte[] buffer, int offset, int length)
    {
        if (length >= m_Buffer.Length || m_BufferPos + length >= m_Buffer.Length)
        {
            OnError("Invalid receive data", null);
            return;
        }

        Array.Copy(buffer, offset, m_Buffer, m_BufferPos, length);
        m_BufferPos += length;

        if (m_BufferPos < Packet.HEADER_SIZE)
            return;

        int remainSize = m_BufferPos;
        int readOffset = 0;

        while (remainSize > 0)
        {
            ushort packetSize = (ushort)(BitConverter.ToUInt16(m_Buffer, readOffset + Packet.PACKET_SIZE_OFFSET) + Packet.HEADER_SIZE);
            if (packetSize < 0 || packetSize >= Packet.BUFFER_SIZE)
            {
                OnError("Invalid receive data", null);
                return;
            }

            if (packetSize > remainSize)
                break;

            remainSize -= packetSize;
            readOffset += packetSize;

            base.OnReceived(m_Buffer, readOffset, packetSize);
        }

        if (readOffset > 0)
        {
            if (m_BufferPos > readOffset)
            {
                Array.Copy(m_Buffer, readOffset, m_Buffer, 0, m_BufferPos - readOffset);
                m_BufferPos -= readOffset;
            }
            else
            {
                m_BufferPos = 0;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;
using BitBox.Log;
using BitBoxExample.CSCommon;

namespace BitBoxAppServerExample.Scripts
{
    public partial class ExampleSession : Session
    {
        private byte[] m_Buffer = new byte[Packet.BUFFER_SIZE];
        private int m_BufferPos = 0;

        public ExampleSession(Server server, Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
            : base(server, socket, recvSAEA, sendSAEA)
        {
        }

        protected override void OnReceived(byte[] buffer, int offset, int length)
        {
            if (length >= Packet.BUFFER_SIZE)
            {
                OnError("Invalid receive data", null);
                return;
            }

            Logger.Debug("m_BufferPos:" + m_BufferPos + " length:" + length);

            Array.Copy(buffer, offset, m_Buffer, m_BufferPos, length);
            m_BufferPos += length;

            if (m_BufferPos >= Packet.HEADER_SIZE)
            {
                ushort packetSize = BitConverter.ToUInt16(m_Buffer, Packet.PACKET_SIZE_OFFSET);
                Logger.Debug("packetSize:" + packetSize);
                if (packetSize < 0 || packetSize >= Packet.BUFFER_SIZE - Packet.HEADER_SIZE)
                {
                    OnError("Invalid receive data", null);
                    return;
                }

                if (packetSize + Packet.HEADER_SIZE <= m_BufferPos)
                {
                    base.OnReceived(m_Buffer, 0, packetSize + Packet.HEADER_SIZE);
                    Array.Copy(m_Buffer, packetSize + Packet.HEADER_SIZE, m_Buffer, 0, m_BufferPos - packetSize + Packet.HEADER_SIZE);
                    m_BufferPos -= packetSize + Packet.HEADER_SIZE;
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Text;

namespace BitBoxExample.CSCommon
{
    public class Packet
    {
        public static readonly ushort BUFFER_SIZE = 10240;
        public static readonly ushort HEADER_SIZE = 4;
        public static readonly ushort ID_OFFSET = 0;
        public static readonly ushort PACKET_SIZE_OFFSET = 2;

        public ProtocolID ID { get; private set; }

        private ushort m_wPacketSize;

        private ushort m_wReadOffSet;
        private ushort m_wWriteOffSet;

        private byte[] m_Data;
        private ushort m_wPacketLen;

        public Packet(ProtocolID id)
        {
            m_Data = new byte[BUFFER_SIZE];
            ID = id;
            WriteID(id);
            m_wPacketSize = 0;
            m_wReadOffSet = 0;
            m_wWriteOffSet = 0;

            m_wPacketLen = (ushort)m_Data.Length;
            WritePacketSize(0);
        }

        public Packet(byte[] data, int length)
        {
            ID = (ProtocolID)BitConverter.ToUInt16(data, ID_OFFSET);

            m_wPacketSize = BitConverter.ToUInt16(data, PACKET_SIZE_OFFSET);
            m_wReadOffSet = 0;
            m_wWriteOffSet = 0;

            if (data.Length < (int)(HEADER_SIZE + m_wPacketSize)) //이상하게 패킷이 오면 응급처리를 한다.
            {
                m_wPacketSize = (ushort)(length - 4);
            }

            m_Data = new byte[length];

            Array.Copy(data, 0, m_Data, 0, length);
            m_wPacketLen = (ushort)length;
        }

        public ushort GetRemainDataSize()
        {
            return (ushort)(m_wPacketSize - m_wReadOffSet);
        }

        public void FinishReadData()
        {
            m_wReadOffSet = m_wPacketSize;
        }

        public void Reset()
        {
            m_wReadOffSet = 0;
        }

        public ProtocolID GetID() { return ID; }

        public void SetID(ProtocolID id)
        {
            ID = id;
            WriteID(id);
        }

        public byte[] GetData() { return m_Data; }

        public ushort GetPacketSize() { return m_wPacketSize; }

        public ushort GetTotalPacketSize() 
        { 
            return (ushort)(HEADER_SIZE + m_wPacketSize);
        }

        private void WriteID(ProtocolID id)
        {
            if (m_Data == null)
                m_Data = new byte[BUFFER_SIZE];

            byte[] data = BitConverter.GetBytes((ushort)id);
            data.CopyTo(m_Data, ID_OFFSET);
        }

        private void WritePacketSize(ushort wPacketSize)
        {
            byte[] data = BitConverter.GetBytes(wPacketSize);
            data.CopyTo(m_Data, PACKET_SIZE_OFFSET);
        }

        private void Write(byte data)
        {
            if (HEADER_SIZE + m_wWriteOffSet + sizeof(byte) > m_wPacketLen)
            {
                return;
            }

            m_Data[HEADER_SIZE + m_wWriteOffSet] = data;
            m_wWriteOffSet += sizeof(byte);
            m_wPacketSize += sizeof(byte);

            WritePacketSize(m_wPacketSize);
        }

        private void Write(byte[] data)
        {
            if (HEADER_SIZE + m_wWriteOffSet + data.Length > m_wPacketLen)
            {
                return;
            }

            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(data);
            }

            data.CopyTo(m_Data, HEADER_SIZE + m_wWriteOffSet);
            m_wWriteOffSet += (ushort)data.Length;
            m_wPacketSize += (ushort)data.Length;

            WritePacketSize(m_wPacketSize);
        }

        public void WriteULong(ulong data) { Write(BitConverter.GetBytes(data)); }
        public void WriteUInt(uint data) { Write(BitConverter.GetBytes(data)); }
        public void WriteUShort(ushort data) { Write(BitConverter.GetBytes(data)); }
        public void WriteDouble(double data) { Write(BitConverter.GetBytes(data)); }
        public void WriteFloat(float data) { Write(BitConverter.GetBytes(data)); }
        public void WriteByte(byte data) { Write(data); }
        public void WriteBool(bool data) { Write(BitConverter.GetBytes(data)); }
        public void WriteShort(short data) { Write(BitConverter.GetBytes(data)); }
        public void WriteLong(long data) { Write(BitConverter.GetBytes(data)); }
        public void WriteInt(int data) { Write(BitConverter.GetBytes(data)); }
        public void WriteString(string data)
        {
            //data = data.Trim();
            byte[] ConvData = System.Text.Encoding.Unicode.GetBytes(data);
            if (ConvData.Length >= 255)
            {
                return;
            }
            WriteByte((byte)ConvData.Length);
            Write(ConvData);
        }
        public void WriteByteData(byte[] data)
        {
            // 문자열을 넘길땐 data를 Encoding.ASCII.GetBytes 변환 사용할것
            WriteUShort((ushort)data.Length);
            Write(data);
        }
        public void WriteDateTime(DateTime data)
        {
            Write(BitConverter.GetBytes(data.ToBinary()));
        }

        public byte ReadByte()
        {
            byte read = m_Data[HEADER_SIZE + m_wReadOffSet];
            m_wReadOffSet += sizeof(byte);
            return read;
        }
        public ulong ReadULong()
        {
            ulong read = BitConverter.ToUInt64(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(ulong);
            return read;
        }
        public uint ReadUInt()
        {
            uint read = BitConverter.ToUInt32(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(uint);
            return read;
        }
        public ushort ReadUShort()
        {
            ushort read = BitConverter.ToUInt16(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(ushort);
            return read;
        }
        public double ReadDouble()
        {
            double read = BitConverter.ToDouble(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(double);
            return read;
        }
        public float ReadFloat()
        {
            float read = BitConverter.ToSingle(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(float);
            return read;
        }
        public bool ReadBool()
        {
            bool read = BitConverter.ToBoolean(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(bool);
            return read;
        }
        public short ReadShort()
        {
            short read = BitConverter.ToInt16(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(short);
            return read;
        }
        public long ReadLong()
        {
            long read = BitConverter.ToInt64(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(long);
            return read;
        }
        public int ReadInt()
        {
            int read = BitConverter.ToInt32(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(int);
            return read;
        }
        public string ReadString()
        {
            byte bLen = ReadByte();

            string s = System.Text.Encoding.Unicode.GetString(m_Data, HEADER_SIZE + m_wReadOffSet, bLen);
            m_wReadOffSet += bLen;

            string ts = s.TrimEnd('\0');
            return ts;
        }
        public DateTime ReadDateTime()
        {
            long read = BitConverter.ToInt64(m_Data, HEADER_SIZE + m_wReadOffSet);
            m_wReadOffSet += sizeof(int);
            return DateTime.FromBinary(read);
        }
    }
}
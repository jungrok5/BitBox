using System;
using System.Collections;
using System.Text;

public class Packet
{
    private static readonly int BUFFER_SIZE = 10240;

    public ushort m_wID;

    public ushort m_wTotalSize;
    public ushort m_wHeaderSize;
    public ushort m_wPacketSize;

    private ushort m_wReadOffSet;
    private ushort m_wWriteOffSet;

    public byte[] m_pData;
    public ushort m_wPacketLen;

    public ushort GetRemainDataSize()
    {
        return (ushort)(m_wPacketSize - m_wReadOffSet);
    }

    public void FinishReadData()
    {
        m_wReadOffSet = m_wPacketSize;
    }

    public Packet(ushort id)
    {
        m_pData = new byte[BUFFER_SIZE];
        m_wID = id;
        WriteID(id);
        m_wHeaderSize = 4;
        m_wPacketSize = 0;
        m_wReadOffSet = 0;
        m_wWriteOffSet = 0;

        m_wPacketLen = (ushort)m_pData.Length;
        m_wTotalSize = m_wHeaderSize;
        m_pData[2] = 0;
        m_pData[3] = 0;
    }
    public Packet(byte[] pData, int length)
    {
        m_wID = BitConverter.ToUInt16(pData, 0);

        m_wHeaderSize = 4;
        m_wPacketSize = BitConverter.ToUInt16(pData, 2);
        m_wTotalSize = (ushort)(m_wHeaderSize + m_wPacketSize);
        m_wReadOffSet = 0;
        m_wWriteOffSet = 0;

        if (pData.Length < (int)m_wTotalSize) //이상하게 패킷이 오면 응급처리를 한다.
        {
            m_wPacketSize = (ushort)(length - 4);
            m_wTotalSize = (ushort)(m_wHeaderSize + m_wPacketSize);

        }

        m_pData = new byte[length];

        Array.Copy(pData, 0, m_pData, 0, length);
        m_wPacketLen = (ushort)length;
    }

    public ushort GetID() { return m_wID; }

    public void SetID(ushort wID)
    {
        m_wID = wID;
        m_wHeaderSize = 4;
        m_wTotalSize = (ushort)(m_wHeaderSize + m_wPacketSize);

        WriteID(wID);
    }

    public ushort GetPacketSize() { return m_wPacketSize; }

    public ushort GetTotalPacketSize() { return m_wTotalSize; }

    public void WriteID(ushort wID)
    {
        if (m_pData == null)
        {
            m_pData = new byte[BUFFER_SIZE];
        }

        byte[] data = BitConverter.GetBytes(wID);
        data.CopyTo(m_pData, 0);
    }

    private void WritePacketSize(ushort wPacketSize)
    {
        byte[] data = BitConverter.GetBytes(wPacketSize);
        data.CopyTo(m_pData, 2);
    }

    private void Write(byte data)
    {
        if (m_wHeaderSize + m_wWriteOffSet + sizeof(byte) > m_wPacketLen)
        {
            return;
        }

        m_pData[m_wHeaderSize + m_wWriteOffSet] = data;
        m_wWriteOffSet += sizeof(byte);
        m_wPacketSize += sizeof(byte);
        m_wTotalSize = (ushort)(m_wHeaderSize + m_wPacketSize);

        WritePacketSize(m_wPacketSize);
    }

    private void Write(byte[] data)
    {
        if (m_wHeaderSize + m_wWriteOffSet + data.Length > m_wPacketLen)
        {
            return;
        }

        if (BitConverter.IsLittleEndian == false)
        {
            Array.Reverse(data);
        }

        data.CopyTo(m_pData, m_wHeaderSize + m_wWriteOffSet);
        m_wWriteOffSet += (ushort)data.Length;
        m_wPacketSize += (ushort)data.Length;
        m_wTotalSize = (ushort)(m_wHeaderSize + m_wPacketSize);

        WritePacketSize(m_wPacketSize);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    public byte ReadByte()
    {
        byte read = m_pData[m_wHeaderSize + m_wReadOffSet];
        m_wReadOffSet += sizeof(byte);
        return read;
    }

    public ulong ReadULong()
    {
        ulong read = BitConverter.ToUInt64(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(ulong);
        return read;
    }

    public uint ReadUInt()
    {
        uint read = BitConverter.ToUInt32(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(uint);
        return read;
    }

    public ushort ReadUShort()
    {
        ushort read = BitConverter.ToUInt16(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(ushort);
        return read;
    }

    public double ReadDouble()
    {
        double read = BitConverter.ToDouble(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(double);
        return read;
    }

    public float ReadFloat()
    {
        float read = BitConverter.ToSingle(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(float);
        return read;
    }

    public bool ReadBool()
    {
        bool read = BitConverter.ToBoolean(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(bool);
        return read;
    }

    public short ReadShort()
    {
        short read = BitConverter.ToInt16(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(short);
        return read;
    }

    public long ReadLong()
    {
        long read = BitConverter.ToInt64(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(long);
        return read;
    }

    public int ReadInt()
    {
        int read = BitConverter.ToInt32(m_pData, m_wHeaderSize + m_wReadOffSet);
        m_wReadOffSet += sizeof(int);
        return read;
    }

    public string ReadString()
    {
        byte bLen = ReadByte();

        string s = System.Text.Encoding.Unicode.GetString(m_pData, m_wHeaderSize + m_wReadOffSet, bLen);
        m_wReadOffSet += bLen;

        string ts = s.TrimEnd('\0');
        return ts;
    }

    public void Reset()
    {
        m_wReadOffSet = 0;
    }
}
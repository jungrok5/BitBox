using UnityEngine;
using System.Collections;
using System.Text;
using System;
using BitBoxUnity.Core;

public class Test : MonoBehaviour 
{
    WebSession session;

    byte[] d1 = Encoding.UTF8.GetBytes("일이삼사오");
    byte[] d2 = BitConverter.GetBytes((int)1000);

    void Start()
    {
        byte[] buffer = new byte[d1.Length + d2.Length];

        Array.Copy(d1, 0, buffer, 0, d1.Length);
        Array.Copy(d2, 0, buffer, d1.Length, d2.Length);

        GameObject go = new GameObject("WebSession");
        session = go.AddComponent<WebSession>();
        
        session.Received += HandleReceive;

        session.Connect("http://localhost", 57778);
        session.Send("Test/TestBinary", buffer, 0, buffer.Length);
    }

    void HandleReceive(byte[] buffer, int offset, int length)
    {
        Debug.Log(buffer.Length);

        byte[] receiveBuffer = new byte[length];
        Array.Copy(buffer, 0, receiveBuffer, 0, length);

        byte[] d11 = new byte[d1.Length];
        Array.Copy(receiveBuffer, 0, d11, 0, d1.Length);

        byte[] d22 = new byte[d2.Length];
        Array.Copy(receiveBuffer, d1.Length, d22, 0, d2.Length);

        Debug.Log(Encoding.UTF8.GetString(d11));
        Debug.Log(BitConverter.ToInt32(d22, 0));
    }
}

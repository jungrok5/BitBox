using UnityEngine;
using System.Collections;
using System;

namespace BitBoxUnity.Core
{
    public class RequestBase
    {
        public ArraySegment<byte> Data;

        public void Write(byte[] data, int offset, int length)
        {
            Data = new ArraySegment<byte>(data, offset, length);
        }
    }
}
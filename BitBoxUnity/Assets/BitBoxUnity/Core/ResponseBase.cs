using UnityEngine;
using System.Collections;

namespace BitBoxUnity.Core
{
    public class ResponseBase<T> where T : RequestBase, new()
    {
        public T Read(byte[] data, int offset, int length)
        {
            T responseData = new T();
            responseData.Write(data, offset, length);
            return responseData;
        }
    }
}
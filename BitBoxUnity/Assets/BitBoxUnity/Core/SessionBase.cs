using UnityEngine;
using System.Collections;

namespace BitBoxUnity.Core
{
    // http://msdn.microsoft.com/ko-kr/library/bew39x2a(v=vs.110).aspx

    // HTTP 통신이랑, TCP 통신 모두 사용하려고 설계중(간단하게)

    // TODO
    // 요녀석이 완성되면 이녀석을 기반으로 포톤이나 유니티빌트인네트워크처럼 RPC 느낌으로 호출하는거 예제로 만들어보자 RPC("OnXX")이렇게하면 서버쪽에서 응답하는것까지도 ㅋ

    public class SessionBase<TIDType> : MonoBehaviour
    {
        public delegate void ConnectHandler();
        public delegate void DisconnectHandler();
        public delegate void ReceiveHandler(byte[] buffer, int offset, int length);
        public delegate void ErrorHandler(string message);

        public ConnectHandler Connected;
        public DisconnectHandler Disconnected;
        public ReceiveHandler Received;
        public ErrorHandler Error;

        protected string RemoteAddress = null;
        protected int Port = -1;

        public virtual void Connect(string remoteAddress, int port)
        {
            RemoteAddress = remoteAddress;
            Port = port;
        }

        public virtual void Disconnect()
        {
            RemoteAddress = null;
            Port = -1;
        }

        public virtual bool IsConnected()
        {
            return false;
        }

        public virtual void Send(TIDType id, byte[] buffer, int offset, int length)
        {
        }

        public virtual void Update()
        {
        }
    }
}
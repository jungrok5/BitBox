using UnityEngine;
using System.Collections;

namespace BitBoxUnity.Core
{
    // http://msdn.microsoft.com/ko-kr/library/bew39x2a(v=vs.110).aspx

    // HTTP 통신이랑, TCP 통신 모두 사용하려고 설계중(간단하게)
    // 둘의 차이점

    // HTTP - 연결하는곳이 URL
    // TCP - 연결하는곳은 IP

    // HTTP - 패킷ID는 곧 URL
    // TCP - 패킷ID는 short숫자

    // HTTP - 데이터 받는것은 코루틴으로 WWW 완료시
    // TCP - 데이터 받는것은 BeginReceive 콜백함수

    // 테스트 해보니 HTTP도 클라이언트에서 보낼때 Packet처럼 바이트배열로 보내면됨 
    //Hashtable headers = new Hashtable();
    //headers.Add("Content-Length", buffer.Length);
    //headers.Add("Content-Type", "application/octet-stream");
    //WWW www = new WWW("http://localhost:26830/test/TestBinToBinary", buffer, headers);
    
    // ASP.NET 서버쪽에서는 커스텀 바이트배열모델바인더로 받고 (아래 참고하여 파일이 아닌 InputStream에서 빼와서 Packet조합할 예정)
    // http://www.codeproject.com/Articles/421638/Model-binding-posted-file-to-byte-array

    // 응답시엔 바이너리로 (OutputStream에 직접쓰기)
    // http://weblogs.asp.net/andrewrea/archive/2010/02/16/a-binarycontentresult-for-asp-net-mvc.aspx


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
using UnityEngine;
using System.Collections;

namespace BitBoxUnity.Core
{
    public class WebSession : SessionBase<string>
    {
        private static readonly int BUFFER_SIZE = 8192;

        protected byte[] m_RecvBuffer;

        public override void Connect(string remoteAddress, int port)
        {
            if (IsConnected() == true)
                return;

            base.Connect(remoteAddress, port);

            m_RecvBuffer = new byte[BUFFER_SIZE];

            if (Connected != null)
                Connected();
        }

        public override void Disconnect()
        {
            if (IsConnected() == false)
                return;

            if (Disconnected != null)
                Disconnected();

            m_RecvBuffer = null;

            base.Disconnect();
        }

        public override bool IsConnected()
        {
            return !string.IsNullOrEmpty(RemoteAddress);
        }

        private IEnumerator ReceiveCallback(WWW www)
        {
            yield return www;
            if (www.error == null)
            {
                // TODO 여기서 패킷객체 생성해서 핸들러로 패스
                if (Received != null)
                    Received(www.bytes, 0, www.bytes.Length);
            }

            www.Dispose();
        }

        public override void Send(string id, byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            base.Send(id, buffer, offset, length);

            // TODO id는 호출할 URL의 일부이겠다 여기서 WWW 코루틴을 돌린다?
            // 사이즈는 Content-Length에 넣고 Cotent-Type을 설정해주자
            // 아래 임시 코드임
            Hashtable headers = new Hashtable();
            headers.Add("Content-Length", length);
            headers.Add("Content-Type", "binary/octet-stream");
            StartCoroutine(ReceiveCallback(new WWW(string.Format("{0}:{1}/{2}", RemoteAddress, Port, id), buffer, headers)));
        }

        public override void Update()
        {
            base.Update();
        }
    }
}


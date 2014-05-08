using UnityEngine;
using System.Collections;

namespace BitBoxUnity.Core
{
    public class WebSession : SessionBase<string>
    {
        private static readonly string CONTENT_LENGTH = "Content-Length";
        private static readonly string CONTENT_TYPE = "Content-Type";
        private static readonly string BINARY_OCTET_STREAM = "binary/octet-stream";
        private static readonly string URL = "http://{0}:{1}/{2}";

        public override void Connect(string remoteAddress, int port)
        {
            if (IsConnected() == true)
                return;

            base.Connect(remoteAddress, port);

            if (Connected != null)
                Connected(string.Format("{0}:{1}", RemoteAddress, Port));
        }

        public override void Disconnect()
        {
            if (IsConnected() == false)
                return;

            if (Disconnected != null)
                Disconnected(string.Format("{0}:{1}", RemoteAddress, Port));

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
                if (Received != null)
                    Received(www.bytes, 0, www.bytes.Length);
            }
            else
            {
                if (Error != null)
                    Error(www.error, null);
            }

            www.Dispose();
        }

        public override void Send(string id, byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            base.Send(id, buffer, offset, length);

            // [주의]
            // 유니티의 WWW클래스는 아래 인자로 넣어주는 buffer의 사이즈를 Content-Length로 넘겨버리기 때문에
            // buffer에는 넘겨주려는 데이터만 담겨있어야한다
            Hashtable headers = new Hashtable();
            headers.Add(CONTENT_LENGTH, length);
            headers.Add(CONTENT_TYPE, BINARY_OCTET_STREAM);
            StartCoroutine(ReceiveCallback(new WWW(string.Format(URL, RemoteAddress, Port, id), buffer, headers)));
        }

        public override void Update()
        {
            base.Update();
        }
    }
}


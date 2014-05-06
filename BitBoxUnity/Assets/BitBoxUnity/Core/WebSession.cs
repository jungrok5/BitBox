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
            // TODO 에러처리 필요

            yield return www;
            if (www.error == null)
            {
                if (Received != null)
                    Received(www.bytes, 0, www.bytes.Length);
            }
            else
            {
                Debug.LogError(www.error);
            }

            www.Dispose();
        }

        public override void Send(string id, byte[] buffer, int offset, int length)
        {
            if (IsConnected() == false)
                return;

            base.Send(id, buffer, offset, length);

            Hashtable headers = new Hashtable();
            headers.Add("Content-Length", length);
            headers.Add("Content-Type", "binary/octet-stream");
            StartCoroutine(ReceiveCallback(new WWW(string.Format("http://{0}:{1}/{2}", RemoteAddress, Port, id), buffer, headers)));
        }

        public override void Update()
        {
            base.Update();
        }
    }
}


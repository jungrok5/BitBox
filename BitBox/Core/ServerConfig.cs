using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBox.Core
{
    public class ServerConfig : IDisposable
    {
        public string Version { get; set; }

        public int SendTimeOut { get; set; }
        public int SendBufferSize { get; set; }
        public int ReceiveBufferSize { get; set; }
        public int MaxConnectionNumber { get; set; }
        public int MaxAcceptOps { get; set; }

        public void Dispose()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BitBox.Core
{
    public class ListenerConfig : IDisposable
    {
        public IPEndPoint EndPoint { get; set; }
        public int BackLog { get; set; }

        public void Dispose()
        {
        }
    }
}

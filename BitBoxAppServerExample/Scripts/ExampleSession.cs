using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;

namespace BitBoxAppServerExample.Scripts
{
    public class ExampleSession : Session
    {
        public ExampleSession(Server server, Socket socket, SocketAsyncEventArgs recvSAEA, SocketAsyncEventArgs sendSAEA)
            : base(server, socket, recvSAEA, sendSAEA)
        {
        }
    }
}

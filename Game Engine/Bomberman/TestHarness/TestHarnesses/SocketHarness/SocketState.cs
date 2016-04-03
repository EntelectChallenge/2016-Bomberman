using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TestHarness.TestHarnesses.SocketHarness
{
    public class SocketState
    {
        public Socket Socket { get; set; }
        public byte[] ByteBuffer { get; set; }
    }
}

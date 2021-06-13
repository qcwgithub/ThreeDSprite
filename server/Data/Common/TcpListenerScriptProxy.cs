using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public class TcpListenerScriptProxy
    {
        public Action<TcpListenerData, SocketAsyncEventArgs> onListenerComplete;
        public Action<TcpListenerData, SocketAsyncEventArgs> onAcceptComplete;
    }
}
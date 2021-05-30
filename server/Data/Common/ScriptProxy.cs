using System;
using System.Net.Sockets;

namespace Data
{
    public class ScriptProxy
    {
        public Action<TcpListenerData, SocketAsyncEventArgs> onTcpListenerComplete;
        public Action<TcpClientData, SocketAsyncEventArgs> onTcpClientComplete;
    }
}
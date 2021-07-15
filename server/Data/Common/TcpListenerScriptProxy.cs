using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public interface ITcpListenerCallback
    {
        // void onListenerComplete(TcpListenerData data, SocketAsyncEventArgs e);
        void onAcceptComplete(TcpListenerData data, SocketAsyncEventArgs e);
    }
}
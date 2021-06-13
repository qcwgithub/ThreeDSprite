using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public class TcpClientScriptProxy
    {
        public Action<TcpClientData, SocketAsyncEventArgs> onSomethingComplete;
        public Action<TcpClientData, SocketAsyncEventArgs> onRecvComplete;
        public Action<TcpClientData, SocketAsyncEventArgs> onSendComplete;
        public Action<TcpClientData, SocketAsyncEventArgs> onConnectComplete;
        public Action<TcpClientData, SocketAsyncEventArgs> onDisconnectComplete;
        public Action<TcpClientData> onCloseComplete; // 模拟的

        public Action<TcpClientData, MsgType, object, Action<ECode, object>> dispatch;
    }
}
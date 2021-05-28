using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

namespace Data
{
    public class TcpData
    {
        public Socket socket;
        public SocketAsyncEventArgs e;
        public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
        public int msgSeq = 1;
        public int socketId = 90000;
        public Action<ISocket, bool> onConnect;
        public Action<ISocket, bool> onDisconnect;
    }
}
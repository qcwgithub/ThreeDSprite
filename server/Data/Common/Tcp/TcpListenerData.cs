using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Data;

namespace Data
{
    public sealed class TcpListenerData
    {
        public ServerBaseData serverData;
        public Socket socket;
        public SocketAsyncEventArgs listenSocketArg;
        public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
        public int msgSeq = 1;
        public int socketId = 90000;
        
        public TcpClientData acceptorData;
        public TcpClientData connectorData;

        // 此函数是多线程，因此必须放在 Data
        public void _eCompleted_multiThreaded(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(_eCompleted_mainThread, e);
        }
        private void _eCompleted_mainThread(object _e)
        {
            this.serverData.scriptProxy.onTcpListenerComplete(this, (SocketAsyncEventArgs)_e);
        }
    }
}
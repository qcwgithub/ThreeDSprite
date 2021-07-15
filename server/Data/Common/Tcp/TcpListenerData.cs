using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Data;

namespace Data
{
    public partial class TcpListenerData
    {
        public bool isForClient;
        public ServerData serverData;
        public Socket socket;
        public SocketAsyncEventArgs listenSocketArg;
        public bool accepting = false;

        // public TcpClientData connectorData;
        // public TcpClientData acceptorData;

        // 此函数是多线程，因此必须放在 Data
        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(onComplete, e);
        }
    }
}
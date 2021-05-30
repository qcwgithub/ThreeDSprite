using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Data;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Data
{
    public sealed class TcpClientData
    {
        public bool isConnector;
        public bool isAcceptor { get { return !this.isConnector; } }

        public Socket _socket;
        public IPEndPoint ipEndPointForConnector; // when isConnector == true
        public CancellationTokenSource _cancellationTaskSource;
        public CancellationToken _cancellationToken;
        public SocketAsyncEventArgs _innArgs;
        public SocketAsyncEventArgs _outArgs;

        // 自定义的 id
        public int socketId;
        public ServerBaseData serverData;

        // when isAcceptor == true
        public bool connectedFromServer;

        public bool connecting;
        public bool connected;
        public bool sending;

        // when isConnectedFromClient == true
        public PMPlayerInfo Player;
        public int clientTimestamp;

        public List<byte[]> sendList = new List<byte[]>();
        public byte[] recvBuffer = new byte[8192];
        public int recvOffset = 0;

        // 这个是多线程调用，因为需要放在 Data 这边
        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(_onTcpClientComplete, e);
        }

        void _onTcpClientComplete(object e)
        {
            this.serverData.scriptProxy.onTcpClientComplete(this, (SocketAsyncEventArgs)e);
        }
    }
}
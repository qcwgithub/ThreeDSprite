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
    public interface ITcpClientScriptProxyProvider
    {
        TcpClientScriptProxy tcpClientScriptProxy { get; }
    }

    public sealed class TcpClientData
    {
        public bool isConnector;
        public bool isAcceptor { get { return !this.isConnector; } }

        public Socket _socket;
        public IPEndPoint ipEndPointForConnector; // when isConnector == true
        // public CancellationTokenSource _cancellationTaskSource;
        // public CancellationToken _cancellationToken;
        public SocketAsyncEventArgs _innArgs;
        public SocketAsyncEventArgs _outArgs;
        public Dictionary<int, Action<ECode, object>> waitingResponses = new Dictionary<int, Action<ECode, object>>();

        // 自定义的 id
        public int socketId;
        public ITcpClientScriptProxyProvider proxyProvider;

        // when isAcceptor == true
        public bool oppositeIsClient;

        public bool connecting;
        public bool connected;
        public bool sending;
        public bool closed;

        // when isConnectedFromClient == true
        public object Player;
        public int clientTimestamp;

        public List<byte[]> sendList = new List<byte[]>();
        public byte[] recvBuffer = new byte[8192];
        public int recvOffset = 0;

        // 这个是多线程调用，因为需要放在 Data 这边
        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(onSomethingComplete, e);
        }

        void onSomethingComplete(object _e)
        {
            if (this.closed)
            {
                return;
            }
            var e = (SocketAsyncEventArgs)_e;
            this.proxyProvider.tcpClientScriptProxy.onSomethingComplete(this, e);
        }
    }
}
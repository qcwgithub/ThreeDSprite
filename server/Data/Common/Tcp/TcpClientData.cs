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
    public abstract class TcpClientData
    {
        public Socket _socket;
        public IPEndPoint ipEndPointForConnect;
        public CancellationTokenSource _cancellationTaskSource;
        public CancellationToken _cancellationToken;
        public SocketAsyncEventArgs _innArgs;
        public SocketAsyncEventArgs _outArgs;
        public abstract bool isMessageFromServer { get; }
        public virtual bool isConnector { get {return false;} }
        public bool connecting = false;

        public int socketId;
        public ServerBaseData serverData { get; private set; }
        public TcpData tcpData
        {
            get
            {
                return this.serverData.tcpData;
            }
        }

        public bool connected;
        public bool sending;
        public TcpClientData(int socketId, ServerBaseData serverData)
        {
            // _cancellationTaskSource = new CancellationTokenSource();
            // _cancellationToken = _cancellationTaskSource.Token;
            // _innArgs = new SocketAsyncEventArgs();
            // _outArgs = new SocketAsyncEventArgs();
            // _innArgs.Completed += _onComplete;
            // _outArgs.Completed += _onComplete;

            // this.socketId = socketId;
            // this.serverData = serverData;
            // this.sending = false;
            // this.connected = false;
            this.serverData.scriptProxy.constructor(socketId);
        }

        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ET.ThreadSynchronizationContext.Instance.Post(_onConnectComplete, e);
                    break;
                case SocketAsyncOperation.Receive:
                    ET.ThreadSynchronizationContext.Instance.Post(_onRecvComplete, e);
                    break;
                case SocketAsyncOperation.Send:
                    ET.ThreadSynchronizationContext.Instance.Post(_onSendComplete, e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    ET.ThreadSynchronizationContext.Instance.Post(_onDisconnectComplete, e);
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        public abstract Task start();

        public bool isConnected()
        {
            return this.connected;
        }

        public int getId()
        {
            return this.socketId;
        }

        public void close()
        {
            throw new NotImplementedException();
        }

        public PMPlayerInfo Player;
        public int clientTimestamp;

        public List<byte[]> sendList = new List<byte[]>();
        public byte[] recvBuffer = new byte[8192];
        public int recvOffset = 0;
    }
}
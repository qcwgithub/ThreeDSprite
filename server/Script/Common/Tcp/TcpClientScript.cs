using System;
using Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Script
{
    public class TcpClientScript : IServerScript<Server>
    {
        public Server server { get; set; }

        #region constructor
        void _initConnectSocket(TcpClientData @this, string url)
        {
            int index = url.LastIndexOf(':');
            string host = url.Substring(0, index);
            string p = url.Substring(index + 1);
            int port = int.Parse(p);

            IPAddress[] addresses = Dns.GetHostAddresses(host);
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    @this.ipEndPointForConnector = new IPEndPoint(address, port);
                    @this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    @this._socket.NoDelay = true;
                    break;
                }
            }
        }

        public TcpClientData connectorConstructor(TcpClientData @this, string url, int socketId, ServerBaseData serverBaseData)
        {
            @this.isConnector = true;
            this._initConnectSocket(@this, url);

            @this._cancellationTaskSource = new CancellationTokenSource();
            @this._cancellationToken = @this._cancellationTaskSource.Token;
            @this._innArgs = new SocketAsyncEventArgs();
            @this._outArgs = new SocketAsyncEventArgs();
            @this._innArgs.Completed += @this._onComplete;
            @this._outArgs.Completed += @this._onComplete;

            @this.socketId = socketId;
            @this.serverData = serverBaseData;
            @this.connectedFromServer = false;

            @this.connecting = false;
            @this.connected = false;
            @this.sending = false;
            return @this;
        }

        public TcpClientData acceptorConstructor(TcpClientData @this, Socket socket,
            int socketId, ServerBaseData serverBaseData,
            bool connectedFromServer)
        {
            @this.isConnector = false;

            @this._socket = socket;
            @this._cancellationTaskSource = new CancellationTokenSource();
            @this._cancellationToken = @this._cancellationTaskSource.Token;
            @this._innArgs = new SocketAsyncEventArgs();
            @this._outArgs = new SocketAsyncEventArgs();
            @this._innArgs.Completed += @this._onComplete;
            @this._outArgs.Completed += @this._onComplete;

            @this.socketId = socketId;
            @this.serverData = serverBaseData;
            @this.connectedFromServer = connectedFromServer;

            @this.connecting = false;
            @this.connected = true;
            @this.sending = false;
            return @this;
        }
        #endregion

        public async Task start(TcpClientData @this)
        {
            if (@this.isConnector)
            {
                await this.connectUntilSuccess(@this);
            }
            else
            {
                this.startRecv(@this);
                this.startSend(@this);
            }
        }

        public void close(TcpClientData @this)
        {

        }

        void onError(TcpClientData @this, string str)
        {
            this.server.logger.Error(str);
        }

        #region basic access
        public bool isConnected(TcpClientData @this)
        {
            return @this.connected;
        }
        public bool isConnecting(TcpClientData @this)
        {
            return @this.connecting;
        }

        public int getId(TcpClientData @this)
        {
            return @this.socketId;
        }
        #endregion

        #region bind player
        public void bindPlayer(TcpClientData @this, PMPlayerInfo player, int clientTimestamp)
        {
            player.socket = @this;
            @this.Player = player;
            @this.clientTimestamp = clientTimestamp;
        }

        public void unbindPlayer(TcpClientData @this, PMPlayerInfo player)
        {
            player.socket = null;
            @this.Player = null;
            @this.clientTimestamp = 0;
        }

        public int getClientTimestamp(TcpClientData @this)
        {
            return @this.clientTimestamp;
        }

        public PMPlayerInfo getPlayer(TcpClientData @this)
        {
            return @this.Player;
        }
        #endregion

        #region encoding
        // return false if failed
        // when seq > 0: msgType
        // when seq < 0: ecode
        public bool decode(string message,
            out int seq,
            out MsgType msgType, out ECode ecode,
            out string msg)
        {
            seq = 0;
            msgType = (MsgType)0;
            ecode = ECode.Error;
            msg = null;

            int i = message.IndexOf('_');
            if (i < 0 || !int.TryParse(message.Substring(0, i), out seq))
            {
                return false;
            }

            int j = message.IndexOf('_', i + 1);
            if (j < 0)
            {
                return false;
            }

            if (seq > 0)
            {
                int iMsgType;
                if (!int.TryParse(message.Substring(i + 1, j - i - 1), out iMsgType))
                {
                    return false;
                }
                msgType = (MsgType)iMsgType;
            }
            else if (seq < 0)
            {
                int iCode;
                if (!int.TryParse(message.Substring(i + 1, j - i - 1), out iCode))
                {
                    return false;
                }
                ecode = (ECode)iCode;
            }
            else
            {
                return false;
            }

            msg = message.Substring(j + 1);
            return true;
        }

        public string encodeReply(int seq, ECode e, string replyMsg)
        {
            if (replyMsg == null)
            {
                replyMsg = "";
            }
            // seq must < 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)e, replyMsg);
            return message;
        }

        public string encodeSend(int seq, MsgType msgType, string msg)
        {
            // seq must > 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)msgType, msg);
            return message;
        }
        #endregion

        #region send
        
        public async Task<MyResponse> sendAsync(TcpClientData @this, MsgType type, object msg)
        {
            var cs = new TaskCompletionSource<MyResponse>();
            this.send(@this, type, msg, (e, r) =>
            {
                bool success = cs.TrySetResult(new MyResponse(e, r));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + type);
                }
            });
            var xxx = await cs.Task;
            return xxx;
        }

        public void send(TcpClientData @this, MsgType type, object msg, Action<ECode, string> cb)
        {
            if (!this.isConnected(@this))
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }
            var str = this.server.JSON.stringify(msg);
            var seq = @this.serverData.tcpListener.msgSeq++;
            var message = encodeSend(seq, type, str);
            if (cb != null)
            {
                @this.serverData.tcpListener.pendingRequests.Add(seq, cb);
            }

            // int length = Encoding.UTF8.GetByteCount(message);
            // var bytes = new byte[length + 4];

            // var bytes = Encoding.UTF8.GetBytes(message);
            // this.sendOnePacket(bytes);
            this.sendOnePacket(@this, message);
        }

        void sendString(TcpClientData @this, string str)
        {
            if (!this.isConnected(@this))
            {
                return;
            }
            // var bytes = Encoding.UTF8.GetBytes(str);
            this.sendOnePacket(@this, str);
        }

        void sendOnePacket(TcpClientData @this, string message)
        {
            int length = Encoding.UTF8.GetByteCount(message);
            var bytes = new byte[length + sizeof(int)];
            BitConverter.TryWriteBytes(new Span<byte>(bytes), (int)bytes.Length);
            Encoding.UTF8.GetBytes(message, 0, message.Length, bytes, sizeof(int));
            // this.sendBuffer.Write(this.packetSizeCache, 0, this.packetSizeCache.Length);
            // this.sendBuffer.Write(bytes, 0, bytes.Length);

            // test
            // string checkMessage = Encoding.UTF8.GetString(bytes, sizeof(int), bytes.Length - sizeof(int));

            @this.sendList.Add(bytes);
            this.startSend(@this);
        }

        void startSend(TcpClientData @this)
        {
            if (!this.isConnected(@this) || @this.sending || @this.sendList.Count == 0)
            {
                return;
            }

            @this.sending = true;

            var bytes = @this.sendList[@this.sendList.Count - 1];
            @this.sendList.RemoveAt(@this.sendList.Count - 1);

            this._sendAsync(@this, bytes, 0, bytes.Length);
        }

        void _sendAsync(TcpClientData @this, byte[] buffer, int offset, int count)
        {
            try
            {
                @this._outArgs.SetBuffer(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
            bool completed = !@this._socket.SendAsync(@this._outArgs);
            if (completed)
            {
                this.onSendComplete(@this);
            }
        }
        
        void onSendComplete(TcpClientData @this)
        {
            @this.sending = false;
            this.startSend(@this);
        }

        #endregion

        #region connect/disconnect

        void onConnectComplete(TcpClientData @this)
        {
            @this.connecting = false;
            @this.connected = true;
            this.server.tcpListenerScript.onConnect(@this);
            this.startRecv(@this);
            this.startSend(@this);
        }

        void _connectAsync(TcpClientData @this)
        {
            @this._outArgs.RemoteEndPoint = @this.ipEndPointForConnector;
            bool completed = !@this._socket.ConnectAsync(@this._outArgs);
            if (completed)
            {
                this.onConnectComplete(@this);
            }
        }

        async Task connectUntilSuccess(TcpClientData @this)
        {
            if (@this.connecting || @this.connected)
                return;

            while (true)
            {
                @this.connecting = true;
                this._connectAsync(@this);

                while (@this.connecting)
                    await Task.Delay(10);

                if (@this.connected)
                    break;
            }
        }

        void onDisconnectComplete(TcpClientData @this)
        {
            @this.connected = false;
            this.server.tcpListenerScript.onDisconnect(@this);
            if (@this.isConnector)
            {
                this.connectUntilSuccess(@this);
            }
        }
        #endregion
        
        #region recv
        void startRecv(TcpClientData @this)
        {
            _recvAsync(@this, @this.recvBuffer, @this.recvOffset, @this.recvBuffer.Length - @this.recvOffset);
        }

        void _recvAsync(TcpClientData @this, byte[] buffer, int offset, int count)
        {
            try
            {
                @this._innArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }

            bool completed = !@this._socket.ReceiveAsync(@this._innArgs);
            if (completed)
            {
                this.onRecvComplete(@this, @this._innArgs);
            }
        }
        
        void onRecvComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.onError(@this, "SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.onError(@this, "ErrorCode.ERR_PeerDisconnect");
                return;
            }

            @this.recvOffset += e.BytesTransferred;
            if (@this.recvOffset >= sizeof(int))
            {
                int length = BitConverter.ToInt32(@this.recvBuffer, 0);
                if (@this.recvOffset >= length)
                {
                    string message = Encoding.UTF8.GetString(@this.recvBuffer, sizeof(int), length - sizeof(int));
                    this.onMsg(@this, message);

                    Array.Copy(@this.recvBuffer, length, @this.recvBuffer, 0, @this.recvOffset - length);
                    @this.recvOffset = 0;
                }
            }

            if (@this.recvOffset >= @this.recvBuffer.Length)
            {
                var newBuffer = new byte[@this.recvBuffer.Length * 2];
                Array.Copy(@this.recvBuffer, newBuffer, @this.recvOffset);
                @this.recvBuffer = newBuffer;
            }

            // continue recv
            this.startRecv(@this);
        }

        #endregion

        void onMsg(TcpClientData @this, string message)
        {
            if (message == "ping")
            {
                // this.server.logger.info("receive ping, send pong");
                this.sendString(@this, "pong");
                return;
            }

            int seq;
            MsgType msgType;
            ECode eCode;
            string msg;
            if (!decode(message, out seq, out msgType, out eCode, out msg))
            {
                Console.WriteLine("Decode message failed, " + message);
                return;
            }

            //// 2 response message
            if (seq < 0)
            {
                // this.server.logger.Info("recv response " + eCode + ", " + msg);

                Action<ECode, string> responseFun;
                if (@this.serverData.tcpListener.pendingRequests.TryGetValue(-seq, out responseFun))
                {
                    @this.serverData.tcpListener.pendingRequests.Remove(-seq);
                    responseFun(eCode, msg);
                }
                else
                {
                    @this.serverData.logger.Error("NO response fun for " + (-seq));
                }
            }

            //// 3 receive message
            else if (seq > 0)
            {
                this.server.tcpListenerScript.onMessage(@this, @this.connectedFromServer, msgType, msg,
                    (ECode e2, string msg2) =>
                    {
                        this.sendString(@this, encodeReply(-seq, e2, msg2));
                    });
            }
            else
            {
                @this.serverData.logger.Error("onMsg wrong seq: " + seq);
            }
        }

        public void onTcpClientComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    {
                        if (e.SocketError != SocketError.Success)
                        {
                            this.onError(@this, "SocketError." + e.SocketError);
                            return;
                        }
                        e.RemoteEndPoint = null;
                        this.onConnectComplete(@this);
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    this.onRecvComplete(@this, e);
                    break;
                case SocketAsyncOperation.Send:
                    {
                        if (e.SocketError != SocketError.Success)
                        {
                            this.onError(@this, "SocketError." + e.SocketError);
                            return;
                        }

                        if (e.BytesTransferred == 0)
                        {
                            this.onError(@this, "ErrorCode.ERR_PeerDisconnect");
                            return;
                        }
                        this.onSendComplete(@this);
                    }
                    break;
                case SocketAsyncOperation.Disconnect:
                    {
                        this.onError(@this, "SocketError." + e.SocketError);
                        this.onDisconnectComplete(@this);
                    }
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }
    }
}
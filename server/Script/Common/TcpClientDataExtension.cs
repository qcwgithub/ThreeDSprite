using System;
using Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Script
{
    public static class TcpClientDataExtension
    {
        public static void constructor(this TcpClientData @this, int socketId)
        {
            @this._cancellationTaskSource = new CancellationTokenSource();
            @this._cancellationToken = @this._cancellationTaskSource.Token;
            @this._innArgs = new SocketAsyncEventArgs();
            @this._outArgs = new SocketAsyncEventArgs();
            @this._innArgs.Completed += @this._onComplete;
            @this._outArgs.Completed += @this._onComplete;

            @this.socketId = socketId;
            @this.sending = false;
            @this.connected = false;
        }
        public static async Task start(this TcpClientData @this)
        {
            await @this.connectUntilSuccess();
        }
        public static void bindPlayer(this TcpClientData @this, PMPlayerInfo player, int clientTimestamp)
        {
            player.socket = @this;
            @this.Player = player;
            @this.clientTimestamp = clientTimestamp;
        }
        public static void unbindPlayer(this TcpClientData @this, PMPlayerInfo player)
        {
            player.socket = null;
            @this.Player = null;
            @this.clientTimestamp = 0;
        }

        public static int getClientTimestamp(this TcpClientData @this)
        {
            return @this.clientTimestamp;
        }

        public static PMPlayerInfo getPlayer(this TcpClientData @this)
        {
            return @this.Player;
        }
        static void onError(this TcpClientData @this, string str)
        {

        }
        // return false if failed
        // when seq > 0: msgType
        // when seq < 0: ecode
        public static bool decode(string message,
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

        public static string encodeReply(int seq, ECode e, string replyMsg)
        {
            if (replyMsg == null)
            {
                replyMsg = "";
            }
            // seq must < 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)e, replyMsg);
            return message;
        }

        public static string encodeSend(int seq, MsgType msgType, string msg)
        {
            // seq must > 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)msgType, msg);
            return message;
        }
        public static void onMessage(this TcpClientData socket, bool fromServer, MsgType type, string msg, Action<ECode, string> reply)
        {
            this.server.onMessage(socket, fromServer, type, msg, reply);
        }
        static void startSend(this TcpClientData _this)
        {
            if (!_this.isConnected() || _this.sending || _this.sendList.Count == 0)
            {
                return;
            }

            _this.sending = true;

            var bytes = _this.sendList[_this.sendList.Count - 1];
            _this.sendList.RemoveAt(_this.sendList.Count - 1);

            _ @this.sendAsync(bytes, 0, bytes.Length);
        }
        static void sendString(this TcpClientData @this, string str)
        {
            if (!@this.isConnected())
            {
                return;
            }
            // var bytes = Encoding.UTF8.GetBytes(str);
            @this.sendOnePacket(str);
        }
        static void sendOnePacket(this TcpClientData @this, string message)
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
            @this.startSend();
        }
        public static void send(this TcpClientData @this, MsgType type, object msg, Action<ECode, string> cb)
        {
            if (!@this.isConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }
            var str = this.server.JSON.stringify(msg);
            var seq = @this.tcpData.msgSeq++;
            var message = encodeSend(seq, type, str);
            if (cb != null)
            {
                @this.tcpData.pendingRequests.Add(seq, cb);
            }

            // int length = Encoding.UTF8.GetByteCount(message);
            // var bytes = new byte[length + 4];

            // var bytes = Encoding.UTF8.GetBytes(message);
            // this.sendOnePacket(bytes);
            @this.sendOnePacket(message);
        }
        static void _sendAsync(this TcpClientData @this, byte[] buffer, int offset, int count)
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
                @this.onSendComplete();
            }
        }
        static void onSendComplete(this TcpClientData @this)
        {
            @this.sending = false;
            @this.startSend();
        }
        static void _onSendComplete(this TcpClientData @this, object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                @this.onError("SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                @this.onError("ErrorCode.ERR_PeerDisconnect");
                return;
            }
            @this.onSendComplete();
        }
        static void onDisconnectComplete(this TcpClientData @this)
        {
            @this.connected = false;
            if (@this.isConnector)
            {
                @this.connectUntilSuccess();
            }
        }
        static async Task connectUntilSuccess(this TcpClientData @this)
        {
            if (@this.connecting || @this.connected)
                return;

            while (true)
            {
                @this.connecting = true;
                @this._connectAsync();

                while (@this.connecting)
                    await Task.Delay(10);

                if (@this.connected)
                    break;
            }
        }
        static void _onDisconnectComplete(this TcpClientData @this, object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            @this.onError("SocketError." + e.SocketError);
            @this.onDisconnectComplete();
        }
        static void _connectAsync(this TcpClientData @this)
        {
            @this._outArgs.RemoteEndPoint = @this.ipEndPointForConnect;
            bool completed = !@this._socket.ConnectAsync(@this._outArgs);
            if (completed)
            {
                @this.onConnectComplete();
            }
        }
        static void onConnectComplete(this TcpClientData @this)
        {

        }
        static void _initConnectSocket(this TcpClientData @this, string host, int port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    @this.ipEndPointForConnect = new IPEndPoint(address, port);
                    @this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    @this._socket.NoDelay = true;
                    break;
                }
            }
        }
        static void _onConnectComplete(this TcpClientData @this, object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                @this.onError("SocketError." + e.SocketError);
                return;
            }
            e.RemoteEndPoint = null;
            @this.onConnectComplete();
        }
        static void _onRecvComplete(this TcpClientData @this, object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                @this.onError("SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                @this.onError("ErrorCode.ERR_PeerDisconnect");
                return;
            }

            @this.onRecvComplete(e.BytesTransferred);
        }
        static void _initAcceptSocket(this TcpClientData @this, Socket _socket)
        {
            @this._socket = _socket;
        }
        static void _recvAsync(this TcpClientData @this, byte[] buffer, int offset, int count)
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
                @this._onRecvComplete(@this._innArgs);
            }
        }
        public static async Task<MyResponse> sendAsync(this TcpClientData @this, MsgType type, object msg)
        {
            var cs = new TaskCompletionSource<MyResponse>();
            @this.send(type, msg, (e, r) =>
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
        static void startRecv(this TcpClientData @this)
        {
            _recvAsync(@this, @this.recvBuffer, @this.recvOffset, @this.recvBuffer.Length - @this.recvOffset);
        }
        static void onRecvComplete(this TcpClientData @this, int bytesTransferred)
        {
            @this.recvOffset += bytesTransferred;
            if (@this.recvOffset >= sizeof(int))
            {
                int length = BitConverter.ToInt32(@this.recvBuffer, 0);
                if (@this.recvOffset >= length)
                {
                    string message = Encoding.UTF8.GetString(@this.recvBuffer, sizeof(int), length - sizeof(int));
                    @this.onMsg(message);

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
            @this.startRecv();
        }
        static void onMsg(this TcpClientData @this, string message)
        {
            if (message == "ping")
            {
                // this.server.logger.info("receive ping, send pong");
                @this.sendString("pong");
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
                if (@this.tcpData.pendingRequests.TryGetValue(-seq, out responseFun))
                {
                    @this.tcpData.pendingRequests.Remove(-seq);
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
                @this.onMessage(@this.isMessageFromServer, msgType, msg,
                    (ECode e2, string msg2) =>
                    {
                        @this.sendString(encodeReply(-seq, e2, msg2));
                    });
            }
            else
            {
                @this.serverData.logger.Error("onMsg wrong seq: " + seq);
            }
        }
    }
}
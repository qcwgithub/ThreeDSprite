using System;
using Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    public abstract class TcpClientScript : TcpClientScriptBasic
    {
        protected abstract IMessagePacker messagePacker { get; }
        protected abstract int nextSocketId { get; }
        protected abstract int nextMsgSeq { get; }

        public abstract void onConnectComplete(TcpClientData @this, SocketAsyncEventArgs e);
        public abstract void onDisconnectComplete(TcpClientData @this, SocketAsyncEventArgs e);
        public virtual void onCloseComplete(TcpClientData @this)
        {
            // timeout all waiting responses
            if (@this.waitingResponses.Count > 0)
            {
                var list = new List<Action<ECode, object>>();
                foreach (var kv in @this.waitingResponses)
                {
                    list.Add(kv.Value);
                }
                @this.waitingResponses.Clear();
                foreach (var reply in list)
                {
                    reply(ECode.Timeout, null);
                }
            }
        }

        #region constructor
        void _initConnectSocket(TcpClientData @this, string ip, int port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(ip);
            foreach (IPAddress address in addresses)
            {
                @this.ipEndPointForConnector = new IPEndPoint(address, port);
                @this._socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                @this._socket.NoDelay = true;
                break;
            }
        }

        public TcpClientData connectorConstructor(TcpClientData @this, string ip, int port, ITcpClientScriptProxyProvider provider)
        {
            @this.isConnector = true;
            this._initConnectSocket(@this, ip, port);

            // @this._cancellationTaskSource = new CancellationTokenSource();
            // @this._cancellationToken = @this._cancellationTaskSource.Token;
            @this._innArgs = new SocketAsyncEventArgs();
            @this._outArgs = new SocketAsyncEventArgs();
            @this._innArgs.Completed += @this._onComplete;
            @this._outArgs.Completed += @this._onComplete;

            @this.proxyProvider = provider;
            @this.socketId = this.nextSocketId;
            @this.oppositeIsClient = false;

            @this.connecting = false;
            @this.connected = false;
            @this.sending = false;
            @this.closed = false;
            return @this;
        }
        #endregion
        public bool isClosed(TcpClientData @this)
        {
            if (@this == null)
            {
                return true;
            }
            return @this.closed;
        }

        // 用于判断客户端是否连接
        public bool isConnected(TcpClientData @this)
        {
            if (@this == null)
            {
                return false;
            }
            return @this.connected;
        }
        public bool isConnecting(TcpClientData @this)
        {
            if (@this == null)
            {
                return false;
            }
            return @this.connecting;
        }

        public int getSocketId(TcpClientData @this)
        {
            return @this.socketId;
        }

        #region send

        public async Task<MyResponse> sendAsync<T>(TcpClientData @this, MsgType type, T msg)
        {
            var cs = new TaskCompletionSource<MyResponse>();
            this.send<T>(@this, type, msg, (e, r) =>
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

        public void send<T>(TcpClientData @this, MsgType msgType, T msg, Action<ECode, object> cb)
        {
            if (!this.isConnected(@this))
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }

            var seq = this.nextMsgSeq;
            if (cb != null)
            {
                @this.waitingResponses.Add(seq, cb);
            }

            // int length = Encoding.UTF8.GetByteCount(message);
            // var bytes = new byte[length + 4];

            // var bytes = Encoding.UTF8.GetBytes(message);
            // this.sendOnePacket(bytes);
            this.sendOnePacket(@this, (int)msgType, msg, seq, cb != null);
        }

        void sendOnePacket<T>(TcpClientData @this, int msgTypeOrECode, T msg, int seq, bool requireResponse)
        {
            var bytes = this.messagePacker.Pack<T>(msgTypeOrECode, msg, seq, requireResponse);
            @this.sendList.Add(bytes);
            this.send(@this);
        }

        public void onSendComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.close(@this, "onTcpClientComplete_SocketAsyncOperation.Send_SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.close(@this, "onTcpClientComplete_SocketAsyncOperation.Send_e.BytesTransferred == 0");
                return;
            }

            this.send(@this);
        }

        #endregion

        #region recv

        public void onRecvComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.close(@this, "onRecvComplete SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.close(@this, "onRecvComplete e.BytesTransferred == 0");
                return;
            }

            @this.recvOffset += e.BytesTransferred;
            if (this.messagePacker.IsCompeteMessage(@this.recvBuffer, 0, @this.recvOffset))
            {
                int offset = 0;
                UnpackResult r = this.messagePacker.Unpack(@this.recvBuffer, ref offset, @this.recvOffset);

                Array.Copy(@this.recvBuffer, offset, @this.recvBuffer, 0, @this.recvOffset - offset);
                @this.recvOffset = 0;
                this.onMsg(@this, r.seq, r.code, r.msg, r.requireResponse);
            }
            // if (@this.recvOffset >= sizeof(int))
            // {
            //     int length = BitConverter.ToInt32(@this.recvBuffer, 0);
            //     if (@this.recvOffset >= length)
            //     {
            //         string message = Encoding.UTF8.GetString(@this.recvBuffer, sizeof(int), length - sizeof(int));
            //         this.onMsg(@this, message);

            //         Array.Copy(@this.recvBuffer, length, @this.recvBuffer, 0, @this.recvOffset - length);
            //         @this.recvOffset = 0;
            //     }
            // }

            if (@this.recvOffset >= @this.recvBuffer.Length)
            {
                var newBuffer = new byte[@this.recvBuffer.Length * 2];
                Array.Copy(@this.recvBuffer, newBuffer, @this.recvOffset);
                @this.recvBuffer = newBuffer;
            }

            // continue recv
            this.recv(@this);
        }

        void onMsg(TcpClientData @this, int seq, int code, object msg, bool requireResponse)
        {
            if (msg != null && msg is string && (string)msg == "ping")
            {
                // this.server.logger.info("receive ping, send pong");
                this.sendOnePacket(@this, 0, "pong", 0, false);
                return;
            }

            // int seq;
            // MsgType msgType;
            // ECode eCode;
            // string msg;
            // if (!decode(message, out seq, out msgType, out eCode, out msg))
            // {
            //     Console.WriteLine("Decode message failed, " + message);
            //     return;
            // }

            //// 3 receive message
            if (seq > 0)
            {
                MsgType msgType = (MsgType)code;
                if (@this.oppositeIsClient && msgType < MsgType.ClientStart)
                {
                    this.logError(@this, "receive invalid message from client! " + msgType.ToString());
                    if (requireResponse)
                    {
                        this.sendOnePacket<MsgNull>(@this, (int)ECode.Exception, null, -seq, false);
                    }
                    return;
                }

                if (!requireResponse)
                {
                    this.tcpClientScriptProxy.dispatch(@this, msgType, msg, null);
                }
                else
                {
                    this.tcpClientScriptProxy.dispatch(@this, msgType, msg,
                            (ECode e2, object msg2) =>
                            {
                                this.sendOnePacket(@this, (int)e2, msg2, -seq, false);
                            });
                }
            }
            //// 2 response message
            else if (seq < 0)
            {
                // this.server.logger.Info("recv response " + eCode + ", " + msg);

                ECode eCode = (ECode)code;

                Action<ECode, object> responseFun;
                if (@this.waitingResponses.TryGetValue(-seq, out responseFun))
                {
                    @this.waitingResponses.Remove(-seq);
                    responseFun(eCode, msg);
                }
                else
                {
                    this.logError(@this, "NO response fun for " + (-seq));
                }
            }

            else
            {
                this.logError(@this, "onMsg wrong seq: " + seq);
            }
        }

        #endregion
    }
}
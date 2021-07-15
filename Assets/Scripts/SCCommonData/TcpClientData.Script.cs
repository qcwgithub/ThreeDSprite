using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data
{
    public partial class TcpClientData
    {
        void onCloseComplete()
        {
            // timeout all waiting responses
            if (this.waitingResponses.Count > 0)
            {
                var list = new List<Action<ECode, object>>();
                foreach (var kv in this.waitingResponses)
                {
                    list.Add(kv.Value);
                }
                this.waitingResponses.Clear();
                foreach (var reply in list)
                {
                    reply(ECode.Timeout, null);
                }
            }
            
            this.callback.onCloseComplete(this);
        }

        #region constructor
        void _initConnectSocket(string ip, int port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(ip);
            foreach (IPAddress address in addresses)
            {
                this.ipEndPointForConnector = new IPEndPoint(address, port);
                this._socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this._socket.NoDelay = true;
                break;
            }
        }

        public void connectorInit(ITcpClientCallback tcpClientCallback, string ip, int port)
        {
            this.callback = tcpClientCallback;
            this.isConnector = true;
            this._initConnectSocket(ip, port);

            // this._cancellationTaskSource = new CancellationTokenSource();
            // this._cancellationToken = this._cancellationTaskSource.Token;
            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this._onComplete;
            this._outArgs.Completed += this._onComplete;

            this.socketId = this.callback.nextSocketId;
            this.oppositeIsClient = false;

            this.connecting = false;
            this.connected = false;
            this.sending = false;
            this.closed = false;
        }

        public void acceptorInit(ITcpClientCallback tcpClientCallback, Socket socket, bool connectedFromClient)
        {
            this.callback = tcpClientCallback;
            this.isConnector = false;

            this._socket = socket;
            // this._cancellationTaskSource = new CancellationTokenSource();
            // this._cancellationToken = this._cancellationTaskSource.Token;
            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this._onComplete;
            this._outArgs.Completed += this._onComplete;

            this.socketId = this.callback.nextSocketId;
            this.oppositeIsClient = connectedFromClient;

            this.connecting = false;
            this.connected = true;
            this.sending = false;
            this.closed = false;
        }

        #endregion

        public bool isClosed()
        {
            if (this == null)
            {
                return true;
            }
            return this.closed;
        }

        // 用于判断客户端是否连接
        public bool isConnected()
        {
            if (this == null)
            {
                return false;
            }
            return this.connected;
        }
        public bool isConnecting()
        {
            if (this == null)
            {
                return false;
            }
            return this.connecting;
        }

        public int getSocketId()
        {
            return this.socketId;
        }

        #region send

        public async Task<MyResponse> sendAsync(MsgType type, object msg)
        {
            var cs = new TaskCompletionSource<MyResponse>();
            this.send(type, msg, (e, r) =>
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

        public void send(MsgType msgType, object msg, Action<ECode, object> cb)
        {
            if (!this.isConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }

            var seq = this.callback.nextMsgSeq;
            if (cb != null)
            {
                this.waitingResponses.Add(seq, cb);
                // Console.WriteLine("++waiting {0} MsgType.{1} seq = {2}", this.waitingResponses.Count, msgType, seq);
            }

            // int length = Encoding.UTF8.GetByteCount(message);
            // var bytes = new byte[length + 4];

            // var bytes = Encoding.UTF8.GetBytes(message);
            // this.sendOnePacket(bytes);
            this.sendOnePacket((int)msgType, msg, seq, cb != null);
        }

        void sendOnePacket(int msgTypeOrECode, object msg, int seq, bool requireResponse)
        {
            var bytes = this.callback.messagePacker.Pack(msgTypeOrECode, msg, seq, requireResponse);
            this.sendList.Add(bytes);
            this.send();
        }

        public void onSendComplete(SocketAsyncEventArgs e)
        {
            this.sending = false;
            if (e.SocketError != SocketError.Success)
            {
                this.close("onTcpClientComplete_SocketAsyncOperation.Send_SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.close("onTcpClientComplete_SocketAsyncOperation.Send_e.BytesTransferred == 0");
                return;
            }

            this.send();
        }

        #endregion

        #region recv

        void onRecvComplete(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.close("onRecvComplete SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.close("onRecvComplete e.BytesTransferred == 0");
                return;
            }

            this.recvOffset += e.BytesTransferred;
            int offset = 0;
            int count = this.recvOffset;
            while (this.callback.messagePacker.IsCompeteMessage(this.recvBuffer, offset, count))
            {
                UnpackResult r = this.callback.messagePacker.Unpack(this.recvBuffer, offset, count);
                this.onMsg(r.seq, r.code, r.msg, r.requireResponse);

                offset += r.totalLength;
                count -= r.totalLength;
            }

            if (offset > 0)
            {
                Array.Copy(this.recvBuffer, offset, this.recvBuffer, 0, count);
                this.recvOffset = count;
            }

            if (this.recvOffset >= this.recvBuffer.Length)
            {
                var newBuffer = new byte[this.recvBuffer.Length * 2];
                Array.Copy(this.recvBuffer, newBuffer, this.recvOffset);
                this.recvBuffer = newBuffer;
            }

            // continue recv
            this.recv();
        }

        void onMsg(int seq, int code, object msg, bool requireResponse)
        {
            if (msg != null && msg is string && (string)msg == "ping")
            {
                // this.server.logger.info("receive ping, send pong");
                this.sendOnePacket(0, "pong", 0, false);
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
                if (this.oppositeIsClient && msgType < MsgType.ClientStart)
                {
                    this.callback.logError(this, "receive invalid message from client! " + msgType.ToString());
                    if (requireResponse)
                    {
                        this.sendOnePacket((int)ECode.Exception, null, -seq, false);
                    }
                    return;
                }

                if (!requireResponse)
                {
                    this.callback.dispatch(this, msgType, msg, null);
                }
                else
                {
                    this.callback.dispatch(this, msgType, msg,
                            (ECode e2, object msg2) =>
                            {
                                // Console.WriteLine("reply -seq = {0}, msgType = {1}", -seq, msgType);
                                this.sendOnePacket((int)e2, msg2, -seq, false);
                            });
                }
            }
            //// 2 response message
            else if (seq < 0)
            {
                // this.server.logger.Info("recv response " + eCode + ", " + msg);

                ECode eCode = (ECode)code;

                Action<ECode, object> responseFun;
                if (this.waitingResponses.TryGetValue(-seq, out responseFun))
                {
                    this.waitingResponses.Remove(-seq);
                    // Console.WriteLine("--waiting {0}, -seq = {1}", this.waitingResponses.Count, seq);
                    responseFun(eCode, msg);
                }
                else
                {
                    this.callback.logError(this, "NO response fun for " + (-seq));
                }
            }

            else
            {
                this.callback.logError(this, "onMsg wrong seq: " + seq);
            }
        }

        #endregion
    }
}
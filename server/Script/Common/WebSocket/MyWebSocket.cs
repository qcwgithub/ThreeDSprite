using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public abstract class MyWebSocket : ISocket
    {
        const int ReceiveChunkSize = 1024;
        const int SendChunkSize = 1024;

        public int socketId { get; protected set; }
        public Server server { get; protected set; }
        public WebSocketData data { get { return this.server.baseData.webSocketData; } }
        public WebSocket socket { get; protected set; }
        protected CancellationTokenSource cancellationTaskSource;
        public MyWebSocket(int socketId, Server server)
        {
            this.socketId = socketId;
            this.server = server;
            this.cancellationTaskSource = new CancellationTokenSource();
        }

        public void send(MsgType type, object msg, Action<ECode, string> cb)
        {
            if (!this.isConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }

            var str = this.server.JSON.stringify(msg);
            var seq = this.data.msgSeq++;
            var message = this.encodeSend(seq, type, str);
            if (cb != null)
            {
                this.data.pendingRequests.Add(seq, cb);
            }
            this.Send(message);
        }
        public async Task<MyResponse> sendAsync(MsgType type, object msg)
        {
            if (!this.isConnected())
            {
                return ECode.NotConnected;
            }
            return await new RequestObject(this.server, this, type, msg).Task;
        }

        public bool isConnected()
        {
            return this.socket != null && this.socket.State == WebSocketState.Open;
        }

        public int getId()
        {
            return this.socketId;
        }

        public void close()
        {

        }


        public Action<MsgType, string, Action<ECode, string>> MessageListener { get; set; }
        public void setCustomMessageListener(Action<MsgType, string, Action<ECode, string>> fun)
        {
            this.MessageListener = fun;
        }

        public void removeCustomMessageListener()
        {
            this.MessageListener = null;
        }

        public PMPlayerInfo Player { get; set; }
        public int clientTimestamp { get; set; }
        public void bindPlayer(PMPlayerInfo player, int clientTimestamp)
        {
            player.socket = this;
            this.Player = player;
            this.clientTimestamp = clientTimestamp;
        }
        public void unbindPlayer(PMPlayerInfo player)
        {
            player.socket = null;
            this.Player = null;
            this.clientTimestamp = 0;
        }

        public int getClientTimestamp()
        {
            return this.clientTimestamp;
        }

        public PMPlayerInfo getPlayer()
        {
            return this.Player;
        }

        protected abstract void doOnDisconnect();

        protected async void startRecv()
        {
            try
            {
                var buffer = new byte[ReceiveChunkSize];
                var builder = new StringBuilder();
                while (this.socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), this.cancellationTaskSource.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            this.doOnDisconnect();
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            builder.Append(str);
                        }
                    }
                    while (!result.EndOfMessage);

                    this.OnMsg(builder.ToString());
                    builder.Clear();
                }
            }
            catch (Exception ex)
            {
                this.doOnDisconnect();
            }
            finally
            {
                this.socket.Dispose();
            }
        }

        protected string encodeReply(int seq, ECode e, string replyMsg)
        {
            if (replyMsg == null)
            {
                replyMsg = "";
            }
            // seq must < 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)e, replyMsg);
            return message;
        }

        protected string encodeSend(int seq, MsgType msgType, string msg)
        {
            // seq must > 0
            string message = string.Format("{0}_{1}_{2}", seq, (int)msgType, msg);
            return message;
        }

        // return false if failed
        // when seq > 0: msgType
        // when seq < 0: ecode
        protected bool decode(string message,
            out int seq,
            out MsgType msgType, out ECode ecode,
            out string msg)
        {
            seq = 0;
            msgType = (MsgType)0;
            ecode = ECode.Error;
            msg = null;

            int i = msg.IndexOf('_');
            if (i < 0 || !int.TryParse(msg.Substring(0, i), out seq))
            {
                return false;
            }

            int j = msg.IndexOf('_', i + 1);
            if (j < 0)
            {
                return false;
            }

            if (seq > 0)
            {
                int iMsgType;
                if (!int.TryParse(msg.Substring(i + 1, j - i - 1), out iMsgType))
                {
                    return false;
                }
                msgType = (MsgType)iMsgType;
            }
            else if (seq < 0)
            {
                int iCode;
                if (!int.TryParse(msg.Substring(i + 1, j - i - 1), out iCode))
                {
                    return false;
                }
                ecode = (ECode)iCode;
            }
            else
            {
                return false;
            }

            msg = msg.Substring(j + 1);
            return true;
        }


        protected async void Send(string message)
        {
            if (this.socket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await this.socket.SendAsync(
                    new ArraySegment<byte>(messageBuffer, offset, count),
                    WebSocketMessageType.Text,
                    lastMessage,
                    this.cancellationTaskSource.Token);
            }
        }

        protected virtual void OnMsg(string data)
        {
            //// 1 ping pong
            if (data == "ping")
            {
                // this.server.logger.info("receive ping, send pong");
                this.Send("pong");
                return;
            }

            int seq;
            MsgType msgType;
            ECode eCode;
            string msg;
            if (!this.decode(data, out seq, out msgType, out eCode, out msg))
            {
                Console.WriteLine("Decode message failed, " + data);
                return;
            }

            //// 2 response message
            if (seq < 0)
            {
                Action<ECode, string> responseFun;
                if (this.data.pendingRequests.TryGetValue(-seq, out responseFun))
                {
                    this.data.pendingRequests.Remove(-seq);
                    responseFun(eCode, msg);
                }
            }

            //// 3 receive message
            else if (seq > 0)
            {
                if (this.MessageListener != null)
                {
                    this.MessageListener(msgType, msg, (ECode e2, string msg2) =>
                    {
                        this.Send(this.encodeReply(-seq, e2, msg2));
                    });
                }
            }
            else
            {
                this.server.logger.Error("onMsg wrong seq: " + seq);
            }
        }
    }
}
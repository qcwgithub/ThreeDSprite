using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using Model;
using System.Collections.Generic;

public abstract class MyTcp : ISocket
{
    public int socketId { get; protected set; }
    public Socket socket { get; protected set; }
    public Server server { get; protected set; }
    public TcpData data { get { return this.server.baseData.tcpData; } }
    protected CancellationTokenSource cancellationTaskSource;
    protected CancellationToken cancellationToken;
    protected SocketAsyncEventArgs innArgs;
    protected SocketAsyncEventArgs outArgs;
    // protected Model.RingBuffer recvBuffer;
    // protected Model.RingBuffer sendBuffer;
    // private Model.TcpPacket parser;
    // private byte[] packetSizeCache;
    // private MemoryStream memoryStream;
    protected bool connected;
    protected bool sending;
    public MyTcp(int socketId, Server server)
    {
        this.socketId = socketId;
        this.server = server;
        this.cancellationTaskSource = new CancellationTokenSource();
        this.cancellationToken = this.cancellationTaskSource.Token;
        this.innArgs = new SocketAsyncEventArgs();
        this.outArgs = new SocketAsyncEventArgs();
        this.innArgs.Completed += this.onComplete;
        this.outArgs.Completed += this.onComplete;
        // this.recvBuffer = new Model.RingBuffer();
        // this.sendBuffer = new Model.RingBuffer();
        // this.packetSizeCache = new byte[4];
        // this.memoryStream = this.data.MemoryStreamManager.GetStream("message", ushort.MaxValue);
        //this.parser = new Model.TcpPacket(NetPacket.PacketSizeLength4, this.recvBuffer, this.memoryStream);
        this.sending = false;
        this.connected = false;
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

    protected void onError(string e)
    {
        this.server.logger.info("ERROR " + e);
    }

    private void onComplete(object sender, SocketAsyncEventArgs e)
    {
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Connect:
                ET.ThreadSynchronizationContext.Instance.Post(this.onConnectComplete, e);
                break;
            case SocketAsyncOperation.Receive:
                ET.ThreadSynchronizationContext.Instance.Post(this.onRecvComplete, e);
                break;
            case SocketAsyncOperation.Send:
                ET.ThreadSynchronizationContext.Instance.Post(this.onSendComplete, e);
                break;
            case SocketAsyncOperation.Disconnect:
                ET.ThreadSynchronizationContext.Instance.Post(this.onDisconnectComplete, e);
                break;
            default:
                throw new Exception($"socket error: {e.LastOperation}");
        }
    }

    #region connect

    protected virtual void onConnectComplete(object o)
    {

    }
    #endregion

    #region send
    private List<byte[]> sendList = new List<byte[]>();
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
        
        // int length = Encoding.UTF8.GetByteCount(message);
        // var bytes = new byte[length + 4];

        // var bytes = Encoding.UTF8.GetBytes(message);
        // this.sendOnePacket(bytes);
        this.sendOnePacket(message);
    }
    public async Task<MyResponse> sendAsync(MsgType type, object msg)
    {
        var cs = new TaskCompletionSource<MyResponse>();
        this.send(type, msg, (e, r) => {
            bool success = cs.TrySetResult(new MyResponse(e, r));
            if (!success)
            {
                Console.WriteLine("!cs.TrySetResult " + type);
            }
        });
        var xxx = await cs.Task;
        return xxx;
    }
    private void sendString(string str)
    {
        if (!this.isConnected())
        {
            return;
        }
        // var bytes = Encoding.UTF8.GetBytes(str);
        this.sendOnePacket(str);
    }
    private void sendOnePacket(string message)
    {
        int length = Encoding.UTF8.GetByteCount(message);
        var bytes = new byte[length + sizeof(int)];
        BitConverter.TryWriteBytes(new Span<byte>(bytes), (int)bytes.Length);
        Encoding.UTF8.GetBytes(message, 0, message.Length, bytes, sizeof(int));
        // this.sendBuffer.Write(this.packetSizeCache, 0, this.packetSizeCache.Length);
        // this.sendBuffer.Write(bytes, 0, bytes.Length);

    // test
    string checkMessage = Encoding.UTF8.GetString(bytes, sizeof(int), bytes.Length - sizeof(int));

        this.sendList.Add(bytes);
        this.startSend();
    }
    protected void startSend()
    {
        if (!this.isConnected() || this.sending || this.sendList.Count == 0)
        {
            return;
        }

        // 没有数据需要发送
        // if (this.sendBuffer.Length == 0)
        // {
        //     this.sending = false;
        //     return;
        // }

        this.sending = true;

        // int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
        // if (sendSize > this.sendBuffer.Length)
        // {
        //     sendSize = (int)this.sendBuffer.Length;
        // }

        // this.sendAsync(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
        var bytes = this.sendList[this.sendList.Count - 1];
        this.sendList.RemoveAt(this.sendList.Count - 1);
        this.outArgs.SetBuffer(bytes, 0, bytes.Length);
        bool completed = !this.socket.SendAsync(this.outArgs);
        if (completed)
        {
            this.onSendComplete(this.outArgs);
        }
    }
    // public void sendAsync(byte[] buffer, int offset, int count)
    // {
    //     try
    //     {
    //         this.outArgs.SetBuffer(buffer, offset, count);
    //     }
    //     catch (Exception e)
    //     {
    //         throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
    //     }
    //     bool completed = !this.socket.SendAsync(this.outArgs);
    //     if (completed)
    //     {
    //         this.onSendComplete(this.outArgs);
    //     }
    // }

    private void onSendComplete(object o)
    {
        this.sending = false;
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }

        if (e.BytesTransferred == 0)
        {
            this.onError("ErrorCode.ERR_PeerDisconnect");
            return;
        }

        // this.sendBuffer.FirstIndex += e.BytesTransferred;
        // if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize)
        // {
        //     this.sendBuffer.FirstIndex = 0;
        //     this.sendBuffer.RemoveFirst();
        // }

        this.startSend();
    }
    #endregion

    #region recv
    private byte[] recvBuffer = new byte[8192];
    private int recvOffset = 0;
    protected void startRecv()
    {
        // int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;
        // this.recvAsync(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
        this.recvAsync(this.recvBuffer, this.recvOffset, this.recvBuffer.Length - this.recvOffset);
    }

    private void recvAsync(byte[] buffer, int offset, int count)
    {
        try
        {
            this.innArgs.SetBuffer(buffer, offset, count);
        }
        catch (Exception e)
        {
            throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
        }

        bool completed = !this.socket.ReceiveAsync(this.innArgs);
        if (completed)
        {
            this.onRecvComplete(this.innArgs);
        }
    }
    private void onRecvComplete(object o)
    {
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }

        if (e.BytesTransferred == 0)
        {
            this.onError("ErrorCode.ERR_PeerDisconnect");
            return;
        }

        // this.recvBuffer.LastIndex += e.BytesTransferred;
        // if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
        // {
        //     this.recvBuffer.AddLast();
        //     this.recvBuffer.LastIndex = 0;
        // }

        this.recvOffset += e.BytesTransferred;
        if (this.recvOffset >= sizeof(int))
        {
            int length = BitConverter.ToInt32(this.recvBuffer, 0);
            if (this.recvOffset >= length)
            {
                string message = Encoding.UTF8.GetString(this.recvBuffer, sizeof(int), length - sizeof(int));
                this.onMsg(message);

                Array.Copy(this.recvBuffer, length, this.recvBuffer, 0, this.recvOffset - length);
                this.recvOffset = 0;
            }
        }
        
        if (this.recvOffset >= this.recvBuffer.Length)
        {
            var newBuffer = new byte[this.recvBuffer.Length * 2];
            Array.Copy(this.recvBuffer, newBuffer, this.recvOffset);
            this.recvBuffer = newBuffer;
        }

        // continue recv
        this.startRecv();
    }
    #endregion

    #region disconnect
    protected abstract void didOnDisconnect();
    protected void onDisconnectComplete(object o)
    {
        this.connected = false;
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
        this.onError("SocketError." + e.SocketError);
        this.didOnDisconnect();
    }
    #endregion

    #region encode/decode
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
    #endregion

    #region handle message
    public Action<MsgType, string, Action<ECode, string>> MessageListener { get; set; }
    public void setCustomMessageListener(Action<MsgType, string, Action<ECode, string>> fun)
    {
        this.MessageListener = fun;
    }

    public void removeCustomMessageListener()
    {
        this.MessageListener = null;
    }

    protected virtual void onMsg(string message)
    {
        if (message == "ping")
        {
            // this.server.logger.info("receive ping, send pong");
            this.sendString("pong");
            return;
        }

        int seq;
        MsgType msgType;
        ECode eCode;
        string msg;
        if (!this.decode(message, out seq, out msgType, out eCode, out msg))
        {
            Console.WriteLine("Decode message failed, " + message);
            return;
        }

        //// 2 response message
        if (seq < 0)
        {
            this.server.logger.info("recv response " + eCode + ", " + msg);

            Action<ECode, string> responseFun;
            if (this.data.pendingRequests.TryGetValue(-seq, out responseFun))
            {
                this.data.pendingRequests.Remove(-seq);
                responseFun(eCode, msg);
            }
            else
            {
                this.server.logger.info("NO response fun for " + (-seq));
            }
        }

        //// 3 receive message
        else if (seq > 0)
        {
            this.server.logger.info("recv message " + msgType + ", " + msg);
            if (this.MessageListener != null)
            {
                this.MessageListener(msgType, msg, (ECode e2, string msg2) =>
                {
                    this.sendString(this.encodeReply(-seq, e2, msg2));
                });
            }
            else
            {
                this.server.logger.info("NO message listener for " + msgType);
            }
        }
        else
        {
            this.server.baseScript.error("onMsg wrong seq: " + seq);
        }
    }
    #endregion
}
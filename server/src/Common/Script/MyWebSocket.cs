using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;

public class MyWebSocket
{
    public int mySocketId = -1;
    public Server server;
    public WebSocket socket;
    public Action<object, bool> onConnect = null;
    public Action<object, bool> onDisconnect = null;

    public object Player = null;
    public int clientTimestamp = 0;
    public Action<MsgType, string, Action<ECode, string>> MessageListener = null;

    protected CancellationTokenSource cancellationTaskSource = new CancellationTokenSource();
    const int ReceiveChunkSize = 1024;
    const int SendChunkSize = 1024;


    public bool IsConnected()
    {
        return this.socket != null && this.socket.State == WebSocketState.Open;
    }

    protected async void startRecv()
    {
        var buffer = new byte[ReceiveChunkSize];
        try
        {
            while (this.socket.State == WebSocketState.Open)
            {
                var stringResult = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), this.cancellationTaskSource.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        this.onDisconnect(this);
                    }
                    else
                    {
                        var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        stringResult.Append(str);
                    }
                }
                while (!result.EndOfMessage);

                this.onMsg(stringResult.ToString());
            }
        }
        catch (Exception ex)
        {
            this.onDisconnect(this);
        }
        finally
        {
            this.socket.Dispose();
        }
    }

    protected string encode_reply(int seq, ECode e, string replyMsg)
    {
        // seq must < 0
        string message = string.Format("{0}_{1}_{2}", seq, (int)e, replyMsg);
        return message;
    }

    protected string encode_send(int seq, MsgType msgType, string msg)
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

    public void send(MsgType type, string msg, Action<ECode, string> cb)
    {        
        var seq = this.server.baseData.msgSeq++;
        var message = this.encode_send(seq, type, msg);
        if (cb != null)
        {
            this.server.baseData.pendingRequests.Add(seq, cb);
        }
        this.send(message);
    }

    protected async void send(string message)
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

    protected virtual void onMsg(string data)
    {
        //// 1 ping pong
        if (data == "ping")
        {
            // this.server.logger.info("receive ping, send pong");
            this.send("pong");
            return;
        }

        int seq;
        MsgType msgType;
        ECode eCode;
        string msg;
        if (!this.decode(data, out seq, out msgType, out eCode, out msg))
        {
            return;
        }

        //// 2 response message
        if (seq < 0)
        {
            Action<ECode, string> responseFun;
            if (this.server.baseData.pendingRequests.TryGetValue(-seq, out responseFun))
            {
                this.server.baseData.pendingRequests.Remove(-seq);
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
                    this.send(this.encode_reply(-seq, e2, msg2));
                });
            }
        }
        else
        {
            this.server.baseScript.error("onMsg wrong seq: " + seq);
        }
    }
}
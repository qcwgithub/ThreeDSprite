using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.IO;

public class TcpData
{
    public Socket socket;
    public SocketAsyncEventArgs e;
    public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
    public int msgSeq = 1;
    public int socketId = 90000;
    public object listenerObject;
    public Dictionary<int, object> connectSockets = new Dictionary<int, object>();
    public RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

    public Action<ISocket, bool> onConnect;
    public Action<ISocket, bool> onDisconnect;
}

public class NetProtoTcp : INetProto, IScript
{
    public Server server { get; set; }
    public TcpData data { get { return this.server.baseData.tcpData; } }

    public string urlForServer(string host, int port)
    {
        var url = //"http://" + 
        host + ":" + port;
        //url += "?sign=" + ServerConst.SERVER_SIGN;
        //url += "&purpose=" + this.server.purpose;
        return url;
    }

    public async Task<ISocket> connectAsync(string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
    {
        var tcp = new MyTcpC(this.data.socketId++, this.server, url, onConnect, onDisconnect);
        await tcp.start();
        return tcp;
    }

    private void onComplete(object sender, SocketAsyncEventArgs e)
    {
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Accept:
                ET.ThreadSynchronizationContext.Instance.Post(this.onAcceptComplete, e);
                break;
            default:
                throw new Exception($"socket accept error: {e.LastOperation}");
        }
    }
    private void accept(SocketAsyncEventArgs e, Socket socket)
    {
        e.AcceptSocket = null;
        bool completed = !socket.AcceptAsync(e);
        if (completed)
        {
            this.onAcceptComplete(e);
        }
    }

    private void onAcceptComplete(object _e)
    {
        var e = (SocketAsyncEventArgs)_e;
        if (e.SocketError != SocketError.Success)
        {
            //Log.Error($"accept error {innArgs.SocketError}");
            this.accept(e, this.data.socket);
            return;
        }

        bool isServer = true; // TODO
        var s = new MyTcpS(this.data.socketId++, this.server, e.AcceptSocket, isServer, this.data.onConnect, this.data.onDisconnect);
        s.start();

        // continue accept
        this.accept(e, this.data.socket);
    }

    public void listen(int port, Func<bool> acceptClient, Action<ISocket, bool> onConnect, Action<ISocket, bool> onDisconnect)
    {
        var socket = this.data.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        socket.Bind(new IPEndPoint(IPAddress.Any, port));
        socket.Listen(1000);

        var e = this.data.e = new SocketAsyncEventArgs();
        e.Completed += this.onComplete;

        this.data.onConnect = onConnect;
        this.data.onDisconnect = onDisconnect;

        this.accept(e, socket);
    }
}
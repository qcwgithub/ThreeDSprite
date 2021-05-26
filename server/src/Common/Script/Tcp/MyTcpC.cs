using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Net;

// 用于服务器连接服务器
public class MyTcpC : MyTcp
{
    string url;
    Action<ISocket> onConnect;
    Action<ISocket> onDisconnect;
    IPEndPoint ipEndPoint = null;

    public MyTcpC(int socketId, Server server, string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
        : base(socketId, server)
    {
        this.url = url;
        this.onConnect = onConnect;
        this.onDisconnect = onDisconnect;

        int index = this.url.LastIndexOf(':');
        string host = this.url.Substring(0, index);
        string p = this.url.Substring(index + 1);
        int port = int.Parse(p);

        IPAddress[] addresses = Dns.GetHostAddresses(host);
        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                this.ipEndPoint = new IPEndPoint(address, port);
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.NoDelay = true;
                break;
            }
        }
    }

    protected override void didOnDisconnect()
    {
        Console.WriteLine("Server disconnect");

        if (this.onDisconnect != null)
            this.onDisconnect(this);

        this.connectUntilSuccess();
    }

    public override async Task start()
    {
        await this.connectUntilSuccess();
    }

    private bool connecting = false;
    protected async Task connectUntilSuccess()
    {
        if (this.connecting || this.connected)
            return;
            
        while (true)
        {
            this.connecting = true;

            this.outArgs.RemoteEndPoint = this.ipEndPoint;
            bool completed = !this.socket.ConnectAsync(this.outArgs);

            if (completed)
                this.onConnectComplete(this.outArgs);

            while (this.connecting)
                await Task.Delay(10);

            if (this.connected)
                break;
        }
    }

    protected override void onConnectComplete(object o)
    {
        this.connecting = false;
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }

        e.RemoteEndPoint = null;
        this.connected = true;
        this.startRecv();
        this.startSend();

        if (this.onConnect != null)
            this.onConnect(this);
    }
}
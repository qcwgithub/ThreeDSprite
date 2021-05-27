using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

// 只用于服务器连接服务器
public class MyWebSocketC : MyWebSocket
{
    public string url { get; protected set; }
    public Action<ISocket> onConnect { get; protected set; }
    public Action<ISocket> onDisconnect { get; protected set; }

    public MyWebSocketC(int socketId, Server server, string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
        : base(socketId, server)
    {
        this.url = url;
        this.onConnect = onConnect;
        this.onDisconnect = onDisconnect;
    }

    protected void callOnConnect()
    {
        if (this.onConnect != null)
        {
            this.onConnect(this);
        }
    }

    protected void callOnDisconnect()
    {
        if (this.onDisconnect != null)
        {
            this.onDisconnect(this);
        }
    }

    protected override void doOnDisconnect()
    {
        Console.WriteLine("Server disconnect");
        this.callOnDisconnect();
        this.connectUntilSuccessAsync();
    }

    public async Task startAsync()
    {
        await this.connectUntilSuccessAsync();
    }
    
    protected async Task<bool> connectOnceAsync()
    {
        try
        {
            var clientSocket = this.socket as ClientWebSocket;
            if (clientSocket == null)
            {
                clientSocket = new ClientWebSocket();
                this.socket = clientSocket;
            }

            await clientSocket.ConnectAsync(new Uri(this.url), this.cancellationTaskSource.Token);

            this.callOnConnect();
            return true;
        }
        catch (Exception ex)
        {
            //this.Server.logger.error("Connect")
            Console.WriteLine("Connect " + this.url + " failed");
            return false;
        }
    }

    protected async Task connectUntilSuccessAsync()
    {
        while (true)
        {
            if (this.socket == null || this.socket.State == WebSocketState.Closed)
            {
                bool success = await this.connectOnceAsync();
                if (success)
                {
                    break;
                }
            }
            await Task.Delay(3000);
        }
    }
}

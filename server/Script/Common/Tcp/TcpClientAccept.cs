using System;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TcpClientAccept : TcpClient
{
    public bool isConnectFromServer { get; protected set; }
    public Action<ISocket, bool> onConnect { get; protected set; }
    public Action<ISocket, bool> onDisconnect { get; protected set; }

    public TcpClientAccept(int socketId, Server server,
        Socket socket, bool isConnectFromServer, Action<ISocket, bool> onConnect, Action<ISocket, bool> onDisconnect)
        : base(socketId, server)
    {
        _initAcceptSocket(socket);
        this.isConnectFromServer = isConnectFromServer;
        this.onConnect = onConnect;
        this.onDisconnect = onDisconnect;
        this.connected = true;
    }

    protected override void onConnectComplete()
    {
        throw new NotImplementedException();
    }

    protected override void onDisconnectComplete()
    {
        base.onDisconnectComplete();
        if (this.onDisconnect != null)
        {
            this.onDisconnect(this, this.isConnectFromServer);
        }
    }

    public override Task start()
    {
        this.startRecv();
        this.startSend();

        if (this.onConnect != null)
        {
            this.onConnect(this, this.isConnectFromServer);
        }
        return Task.CompletedTask;
    }
}
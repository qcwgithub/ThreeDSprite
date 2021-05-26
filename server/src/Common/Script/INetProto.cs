using System;
using System.Threading.Tasks;

public interface INetProto
{
    string urlForServer(string host, int port);

    Task<ISocket> connectAsync(string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect);
    void listen(int port, Func<bool> acceptClient, Action<ISocket, bool> onConnect, Action<ISocket, bool> onDisconnect);
}
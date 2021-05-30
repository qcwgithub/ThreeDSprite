using System;
using System.Threading.Tasks;

namespace Data
{
    public interface INetProto
    {
        string urlForServer(string host, int port);

        Task<TcpClientData> connectAsync(string url, Action<TcpClientData> onConnect, Action<TcpClientData> onDisconnect);
        void listen(int port, Func<bool> acceptClient, Action<TcpClientData, bool> onConnect, Action<TcpClientData, bool> onDisconnect);
    }
}
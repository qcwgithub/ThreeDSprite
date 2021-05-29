using System.Net.Sockets;

namespace Data
{
    public interface IScriptProxy
    {
        void onListenSocketComplete(SocketAsyncEventArgs e);
        void onAcceptComplete(SocketAsyncEventArgs e);
        void onConnect(bool isConnector, TcpClientData tcpClientData);
        void onDisconnect(bool isConnector, TcpClientData tcpClientData);
    }
}
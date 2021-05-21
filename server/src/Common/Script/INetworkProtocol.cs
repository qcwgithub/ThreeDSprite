using System;
public interface INetworkProtocol {
    void bindPlayerAndSocket(PMPlayerInfo player, object socket, int clientTimestamp);
    void unbindPlayerAndSocket(PMPlayerInfo player, object socket);
    int getSocketClientTimestamp(object socket);
    PMPlayerInfo getPlayer(object socket);

    string urlForServer(string host, int port);

    string getSocketId(object socket);
    void closeSocket(object socket);
    void send(object socket, MsgType type, object msg, Action<MyResponse> cb);

    object connect(string url, Action<object> onConnect, Action<object> onDisconnect);
    bool isConnected(object socket);
    void listen(int port, Func<bool> acceptClient, Action<object, bool> onConnect, Action<object, bool> onDisconnect);

    void removeCustomMessageListener(object socket);
    void setCustomMessageListener(object socket, Action<MsgType, object, Action<MyResponse>> fun);
}

public class NetworkProtocolBase {
    public void bindPlayerAndSocket(PMPlayerInfo player, object socket, int clientTimestamp) {
        player.socket = socket;
        socket[MagicValue.SOCKET_PLAYER] = player;
        socket[MagicValue.SOCKET_CLIENT_TIMESTAMP] = clientTimestamp;
    }
    public void unbindPlayerAndSocket(PMPlayerInfo player, object socket) {
        player.socket = null;
        socket[MagicValue.SOCKET_PLAYER] = null;
        socket[MagicValue.SOCKET_CLIENT_TIMESTAMP] = 0;
    }
    public int getSocketClientTimestamp(object socket) {
        return socket[MagicValue.SOCKET_CLIENT_TIMESTAMP];
    }
    public PMPlayerInfo getPlayer(object socket) {
        return socket[MagicValue.SOCKET_PLAYER];
    }
}
// using System;
// public interface INetworkProtocol
// {
//     void bindPlayerAndSocket(PMPlayerInfo player, object socket, int clientTimestamp);
//     void unbindPlayerAndSocket(PMPlayerInfo player, object socket);
//     int getSocketClientTimestamp(object socket);
//     PMPlayerInfo getPlayer(object socket);

//     string urlForServer(string host, int port);

//     int getSocketId(object socket);
//     void closeSocket(object socket);
//     void send(object socket, MsgType type, object msg, Action<MyResponse> cb);

//     Task<object> connectAsync(string url, Action<object> onConnect, Action<object> onDisconnect);
//     bool isConnected(object socket);
//     void listen(int port, Func<bool> acceptClient, Action<object, bool> onConnect, Action<object, bool> onDisconnect);

//     void removeCustomMessageListener(object socket);
//     void setCustomMessageListener(object socket, Action<MsgType, object, Action<MyResponse>> fun);
// }

// public class NetworkProtocolBase
// {
//     public void bindPlayerAndSocket(PMPlayerInfo player, object socket, int clientTimestamp)
//     {
//         player.socket = socket;
//         var wrap = socket as WebSocketWrap;
//         wrap.Player = player;
//         wrap.clientTimestamp = clientTimestamp;
//     }
//     public void unbindPlayerAndSocket(PMPlayerInfo player, object socket)
//     {
//         player.socket = null;
//         var wrap = socket as WebSocketWrap;
//         wrap.Player = null;
//         wrap.clientTimestamp = 0;
//     }
//     public int getSocketClientTimestamp(object socket)
//     {
//         var wrap = socket as WebSocketWrap;
//         return wrap.clientTimestamp;
//     }
//     public PMPlayerInfo getPlayer(object socket)
//     {
//         var wrap = socket as WebSocketWrap;
//         return wrap.Player as PMPlayerInfo;
//     }
// }
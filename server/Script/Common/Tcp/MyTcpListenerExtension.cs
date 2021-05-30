// using System;
// using System.Net.Sockets;
// using Data;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Net;
// using System.Text;

// namespace Script
// {
//     public static class MyTcpListenerExtension
//     {
//         /////////////////////// connect ///////////////////////////

//         public static string urlForServer(this TcpListenerData @this, string host, int port)
//         {
//             var url = //"http://" + 
//             host + ":" + port;
//             //url += "?sign=" + ServerConst.SERVER_SIGN;
//             //url += "&purpose=" + this.server.purpose;
//             return url;
//         }

//         public static async Task<TcpClientData> connectAsync(this TcpListenerData @this, string url)
//         {
//             @this.connectorData = new TcpClientData().connectorConstructor(url, @this.socketId++, @this.serverData);
//             await @this.connectorData.start();
//             return @this.connectorData;
//         }

//         static string msgOnConnect = null;
//         static string msgOnDisconnect = null;

//         static string msgOnConnect_true = null;
//         static string msgOnConnect_false = null;
//         static string msgOnDisconnect_true = null;
//         static string msgOnDisconnect_false = null;

//         static void initMsgConnect()
//         {
//             if (msgOnConnect == null)
//             {
//                 msgOnConnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
//                 msgOnDisconnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
//                 msgOnConnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
//                 msgOnConnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
//                 msgOnDisconnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
//                 msgOnDisconnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
//             }
//         }
//         public static void onConnect(this TcpListenerData @this, TcpClientData tcpClient)
//         {
//             initMsgConnect();
//             if (tcpClient.isConnector)
//             {
//                 this.server.dispatcher.dispatch(tcpClient, MsgType.OnConnect, msgOnConnect, null);
//             }
//             else
//             {
//                 this.server.dispatcher.dispatch(tcpClient, MsgType.OnConnect, tcpClient.connectedFromServer ? msgOnConnect_true : msgOnConnect_false, null);
//             }
//         }

//         public static void onDisconnect(this TcpListenerData @this, TcpClientData tcpClient)
//         {
//             initMsgConnect();
//             if (tcpClient.isConnector)
//             {
//                 this.server.dispatcher.dispatch(tcpClient, MsgType.OnDisconnect, msgOnDisconnect, null);
//             }
//             else
//             {
//                 this.server.dispatcher.dispatch(tcpClient, MsgType.OnDisconnect, tcpClient.connectedFromServer ? msgOnDisconnect_true : msgOnDisconnect_false, null);
//             }
//         }

//         /////////////////////// accept ///////////////////////////

//         public static void listen(this TcpListenerData @this, int port, Func<bool> acceptClient)
//         {
//             var socket = @this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//             var e = @this.listenSocketArg = new SocketAsyncEventArgs();
//             e.Completed += @this._eCompleted_multiThreaded;

//             socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

//             socket.Bind(new IPEndPoint(IPAddress.Any, port));
//             socket.Listen(1000);

//             @this.acceptAsync(e);
//         }

//         public static void onTcpListenerComplete(this TcpListenerData @this, SocketAsyncEventArgs e)
//         {
//             switch (e.LastOperation)
//             {
//                 case SocketAsyncOperation.Accept:
//                     @this.onTcpListenerAcceptComplete(e);
//                     break;
//                 default:
//                     throw new Exception($"socket accept error: {e.LastOperation}");
//             }
//         }

//         private static void acceptAsync(this TcpListenerData @this, SocketAsyncEventArgs e)
//         {
//             e.AcceptSocket = null;
//             bool completed = !@this.socket.AcceptAsync(e);
//             if (completed)
//             {
//                 // this.onAcceptComplete(e); // moved to script
//                 @this.onTcpListenerAcceptComplete(e);
//             }
//         }

//         static void onTcpListenerAcceptComplete(this TcpListenerData @this, SocketAsyncEventArgs e)
//         {
//             if (e.SocketError != SocketError.Success)
//             {
//                 //Log.Error($"accept error {innArgs.SocketError}");
//                 @this.acceptAsync(e);
//                 return;
//             }

//             bool isServer = true; // TODO
//             @this.acceptorData = new TcpClientData().acceptorConstructor(e.AcceptSocket, @this.socketId++, @this.serverData, isServer);
//             @this.acceptorData.start();

//             // continue accept
//             @this.acceptAsync(e);
//         }
//     }
// }
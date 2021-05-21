// // // // // // // // imp// // public class NetworkProtocolSocketIO : NetworkProtocolBase : INetworkProtocol, IScript {
//     public Server server;
//     public string urlForServer(string host, int port) {
//         var url = "http://" + host + ":" + port + "/hermes_server?sign=" + ServerConst.SERVER_SIGN;
//         return url;
//     }
//     public string urlForClient(string host, int port) {
//         var url = "https://" + host + ":" + port + "/hermes_client?sign=" + ServerConst.CLIENT_SIGN;
//         return url;
//     }
//     public string getSocketId(socket: socketio.Socket) {
//         return socket.id;
//     }
//     closeSocket(socket: socketio.Socket): void {
//         socket.disconnect(); // 期望是走到 PMOnDisconnect
//     }
//     send(socket: socketio.Socket, type: MsgType, object msg, cb: (r: MyResponse) => void): void {
//         socket.send(type, msg, cb);
//     }
//     connect(string url, onConnect: (socket: socketio.Socket)=>void, onDisconnect: (object socket)=>void): void {
//         // {
//         //     // WARNING: in that case, there is no fallback to long-polling
//         //     transports: [ "websocket" ] // or [ "websocket", "polling" ], which is the same thing
//         // }
//         var s = io(url, {
//             public 2000 reconnectionDelayMax, // 连得快一点
//             public 3000 timeout, // 3秒连不上就算失败，默认是20秒太久了

//             // these two are important
//             // secure: true,
//             // rejectUnauthorized: false,
//         });

//         // 这里不用 off("connect)，因为这里只会进来一次，之后走的是重连逻辑


//         s.on("connect", (socket: socketio.Socket) => onConnect(socket));
//         s.on("disconnect", (socket: socketio.Socket) => onDisconnect(socket));

//         s.connect();
//     }
//     isConnected(object socket): boolean {
//         return (socket as socketio.Socket).connected;
//     }
//     listen(int port, onConnect: (object socket, boolean isServer)=>void, onDisconnect: (object socket, boolean isServer)=>void): void {
//         // const app = express();
//         // app.set("port", port);

//         // var homeDir = require("os").homedir();
//         //console.log("homeDir: " + homeDir);

//         // var keyFile = homeDir + "/config/key.pem";
//         // var certFile = homeDir + "/config/cert.pem";
//         // console.log("keyFile " + keyFile);
//         // console.log("certFile " + certFile);

//         var https = require("http").createServer();
    
//         // var http = require("http").Server(app);
//         var io = socketio(https, {
//             public false perMessageDeflate, // 不要压缩，提升性能
//             public 64 maxHttpBufferSize*1024, // 一个包最大多少 64k
//         });

//         ////////// server /////////////
//         socketio serverNs.Namespace = io.of("/hermes_server");
//         serverNs.use((socket, next) => {
//             var q = socket.handshake.query;
//             if (q.sign !== ServerConst.SERVER_SIGN) {
//                 next(new Error("server - authentication error"));
//             }
//             else {
//                 next();
//             }
//         });
//         serverNs.on("connect", (socket: socketio.Socket) => {
//             onConnect(socket, true);

//             socket.on("disconnect", () => {
//                 onDisconnect(socket, true);
//             });
//         });
//         // serverNs.on("disconnect", (socket: socketio.Socket) => {
//         //     this.dispatcher.dispatch(MsgType.OnDisconnect, { socket: socket, isListen: true, isServer: true }, null);
//         // });

//         ////////// client /////////////
//         socketio clientNs.Namespace = io.of("/hermes_client");
//         clientNs.use((socket, next) => {
//             if (!this.server.baseScript.isReadyForClient()) {
//                 this.server.logger.warn("client - server not ready!");
//                 next(new Error("client - server not ready!"));
//                 return;
//             }
//             var q = socket.handshake.query;
//             if (q.sign !== ServerConst.CLIENT_SIGN) {
//                 this.server.logger.warn("client - authentication error");
//                 next(new Error("client - authentication error"));
//             }
//             else {
//                 next();
//             }
//         });
//         clientNs.on("connect", (socket: socketio.Socket) => {
//             onConnect(socket, false);

//             socket.on("disconnect", () => {
//                 onDisconnect(socket, false);
//             });
//         });
//         // clientNs.on("disconnect", (socket: socketio.Socket) => {
//         //     this.dispatcher.dispatch(MsgType.OnDisconnect, { socket: socket, isListen: true, isServer: false }, null);
//         // });

//         https.listen(port, () => {
//             this.server.logger.info("listening on *: " + port);
//         });
//     }

//     removeAllListeners(socket: socketio.Socket): void {
//         socket.removeAllListeners();
//     }
//     public removeMessageListener(object s) {
//         var s2 = s as any;
//         var tmp = s2[MagicValue.SOCKET_MSG_FUN];
//         if (tmp != null) {
//             delete s2[MagicValue.SOCKET_MSG_FUN];
//             s.off("message", tmp);
//         }
//     }
//     public addMessageListener(object s, fun: (type: MsgType, object msg, reply: (r: MyResponse) => void) => void) {
//         var s2 = s as any;
//         s2[MagicValue.SOCKET_MSG_FUN] = fun;
//         s.on("message", fun);
//     }
// }
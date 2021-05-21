// // // // // // // // // // // // // 文档
// // //     server: Server;
//     public string urlForServer(string host, int port) {
//         var url = "ws://" + host + ":" + port;
//         return url;
//     }
//     public string urlForClient(string host, int port) {
//         var url = "ws://" + host + ":" + port;
//         return url;
//     }
//     public string getSocketId(socket: WebSocket.connection) {
//         var s = socket as any;
//         if (s[MagicValue.SOCKET_ID] == undefined) {
//             s[MagicValue.SOCKET_ID] = this.server.baseData.socketId++;
//         }
//         return s[MagicValue.SOCKET_ID];
//     }
//     closeSocket(socket: WebSocket.connection) {
//         socket.close();
//     }
//     connect(string url, onConnect: (socket: WebSocket.connection)=>void, onDisconnect: (socket: WebSocket.connection)=>void): void {
//         var client = new WebSocket.client({});
//         client.connect(url, null, ServerConst.SERVER_SIGN);
//         client.on("connect", (socket: WebSocket.connection) => {
//             onConnect(socket);

//             socket.on("close", (int code, string desc) => {
//                 onDisconnect(socket);
//             })
//         });
//         client.on("connectFailed", (err: Error) => {
//             console.info("connectFailed, err " + err);
//         });
//     }
//     send(socket: WebSocket.connection, type: MsgType, object msg, cb: (r: MyResponse) => void): void {
//         var _package = msg;

//         _package[MagicValue.MSG_TYPE] = type;

//         var seq = this.server.baseData.msgSeq++;
//         _package[MagicValue.MSG_SEQ] = seq;

//         var str = JSON.stringify(_package);
//         socket.sendUTF(str, (err: Error) => {
//             if (err != null) {
//                 cb(new MyResponse(ECode.WebSocketError, err));
//             }
//             else {
//                 this.server.baseData.pendingRequests[seq] = cb;
//             }
//         });
//     }
//     isConnected(socket: WebSocket.connection): boolean {
//         if (socket == null) {
//             return false;
//         }
//         return socket.connected;
//     }
//     listen(int port, onConnect: (object socket, boolean isServer)=>void, onDisconnect: (object socket, boolean isServer)=>void): void {
//         var http = require("http").createServer();
//         var server = new WebSocket.server({
//             httpServer: http,
//             autoAcceptConnections: true,
//             // keepaliveInterval: 20000,
//         });
//         server.on("request", (request: WebSocket.request) => {
//             if (request.origin == ServerConst.SERVER_SIGN) {
//                 request.accept();
//             }
//             else if (request.origin == ServerConst.CLIENT_SIGN) {
//                 request.accept();
//             }
//             else {
//                 console.info("request.origin = " + request.origin);
//                 request.reject(404, "reject by qiucw");
//             }
//         });
//         server.on("connect", (conn: WebSocket.connection) => {
//             onConnect(conn, true);
//         });
//         server.on("close", (conn: WebSocket.connection, int reason, string desc) => {
//             onDisconnect(conn, true);
//         });
//         http.listen(port, () => {
//             this.server.logger.info("listening on *: " + port);
//         });
//     }
//     removeAllListeners(socket: WebSocket.connection): void {
//         socket.removeAllListeners();
//     }
//     removeMessageListener(socket: WebSocket.connection): void {
//         var s2 = socket as any;
//         var tmp = s2[MagicValue.SOCKET_MSG_FUN];
//         if (tmp != null) {
//             delete s2[MagicValue.SOCKET_MSG_FUN];
//             socket.off("message", tmp);
//         }
//     }
//     addMessageListener(socket: WebSocket.connection, fun: (type: MsgType, object msg, reply: (r: MyResponse) => void) => void): void {
//         var _onMsg = (wsMsg: WebSocket.IMessage) => {
//             var package1 = JSON.parse(wsMsg.utf8Data);
            
//             var seq = package1[MagicValue.MSG_SEQ];
//             delete package1[MagicValue.MSG_SEQ];

//             var msg = package1;

//             if (seq < 0) {
//                 var reponseFun = this.server.baseData.pendingRequests[-seq];
//                 if (reponseFun != null) {
//                     delete this.server.baseData.pendingRequests[-seq];
//                     reponseFun(msg);
//                 }
//             }
//             else if (seq > 0) {
//                 MsgType type = package1[MagicValue.MSG_TYPE];
//                 delete package1[MagicValue.MSG_TYPE];

//                 fun(type, msg, (r: MyResponse) => {
//                     var object package2 = r;
//                     package2[MagicValue.MSG_SEQ] = -seq;
//                     socket.sendUTF(JSON.stringify(package2), null);
//                 });
//             }
//         };
        
//         (socket as any)[MagicValue.SOCKET_MSG_FUN] = _onMsg;
//         socket.on("message", _onMsg);
//     }
// }
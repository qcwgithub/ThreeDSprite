using System;
using System.Net.WebSockets;

public class WebSocketWrap
{
    public Server server;
    public WebSocket socket;

    protected virtual void onMsg(WebSocket.Data data)
    {
        if (typeof(data) != "string")
        {
            this.server.baseScript.error("_onMsg data type " + typeof(data));
            return;
        }

        //// 1 ping pong
        if (data == "ping")
        {
            // this.server.logger.info("receive ping, send pong");
            this.socket.send("pong");
            return;
        }

        object package1 = null;
        try
        {
            package1 = this.server.JSON.parse(data as string);
        }
        catch (Exception ex)
        {
            this.server.baseScript.error("_onMsg JSON error ", ex);
            return;
        }

        var seq = package1[MagicValue.MSG_SEQ];
        delete package1[MagicValue.MSG_SEQ];

        var msg = package1;

        //// 2 response message
        if (seq < 0)
        {
            var reponseFun = this.server.baseData.pendingRequests[-seq];
            if (reponseFun != null)
            {
                delete this.server.baseData.pendingRequests[-seq];
                reponseFun(msg);
            }
        }

        //// 3 receive message
        else if (seq > 0)
        {
            MsgType type = package1[MagicValue.MSG_TYPE];
            delete package1[MagicValue.MSG_TYPE];

            var fun = this.socket[MagicValue.SOCKET_CUS_MSG_FUN];
            if (fun != null)
            {
                fun(type, msg, (MyResponse r) =>
                {
                    var package2 = r;
                    package2[MagicValue.MSG_SEQ] = -seq;
                    if (this.server.baseData.replyServerTime)
                    {
                        package2[MagicValue.MSG_SERVERTIME] = new Date().getTime();
                    }
                    this.socket.send(JSON.stringify(package2), null);
                });
            }
        }
        else
        {
            this.server.baseScript.error("_onMsg wrong seq: " + seq);
        }
    }

    public void initMessageListener()
    {
        var _onMsg = (WebSocket.Data data) => this.onMsg(data);
        // 这句是不是造成内存泄漏？
        // (this.socket as any)[MagicValue.SOCKET_MSG_FUN] = _onMsg;
        this.socket.on("message", _onMsg);
    }
    // removeMessageListner() {
    //     var _onMsg = (this.socket as any)[MagicValue.SOCKET_MSG_FUN];
    //     if (_onMsg != null) {
    //         this.socket.off("message", _onMsg);
    //     }
    // }
    public void closeImmediately()
    {
        // https://github.com/websockets/ws/issues/891
        // 注：this.socket.close 及 terminate 很慢才会触发 "close" 事件
        // 在这段期间内如果客户端重连上来了，会导致 2 个 socket 事件混乱，因此在这里提前发送 "close" 事件。
        // 在 "close" 事件处理程序中会 removeAllListeners，所以之后再出 "close" 事件会被忽略
        //...目前 PMPlayerLogin 也配合检查 OldSocket
        if (this.socket != null)
        {
            this.socket.emit("closeImmediately");
            this.socket.close();
        }
    }
}

public class WebSocketWrapS : WebSocketWrap
{
    // 这些断线检测是参考 socket.io-client
    public int pingInterval = 10000;
    private int pingTimer = -1;

    public int pingTimeout = 3000;
    public int pingTimeoutTimer = -1;
    private void onHearbeat(int timeout)
    {
        this.server.baseScript.clearTimer(this.pingTimeoutTimer);
        this.pingTimeoutTimer = this.server.baseScript.setTimer(() =>
        {
            if (this.socket.readyState == WebSocket.CLOSED)
            {
                return;
            }
            this.server.logger.debug("ping pong timeout, close!!");
            this.closeImmediately();
        }, timeout);
    }

    public void setPing()
    {
        // this.server.logger.info("setPing " + this.pingInterval);
        this.server.baseScript.clearTimer(this.pingTimer);
        this.pingTimer = this.server.baseScript.setTimer(() =>
        {
            // this.server.logger.info("send ping...");
            this.socket.send("ping");
            this.onHearbeat(this.pingTimeout);
        }, this.pingInterval);
    }

    // override
    protected override void onMsg(WebSocket.Data data)
    {
        this.onHearbeat(this.pingInterval + this.pingTimeout);
        if (data == "pong")
        {
            // this.server.logger.info("receive pong...");
            this.setPing();
        }
        else
        {
            base.onMsg(data);
        }
    }
}

public class WebSocketOptions
{
    public bool rejectUnauthorized;
    public bool perMessageDeflate;
    public bool noServer;
}

// 因为需要处理重连，只好包装一下
// only for connect
public class WebSocketWrapC : WebSocketWrap
{
    public string url = null;
    public Action<WebSocketWrap> onConnect = null;
    public Action<WebSocketWrap> onDisconnect = null;
    public int reconnectInterval = 3000;
    public void open()
    {
        if (this.reconnectTimer != -1)
        {
            this.server.baseScript.clearTimer(this.reconnectTimer);
            this.reconnectTimer = -1;
        }

        var clientSocket = new ClientWebSocket();
        this.socket = clientSocket;
        // clientSocket.ConnectAsync
        
        // , new WebSocketOptions
        // {
        //     // origin: ServerConst.SERVER_SIGN,
        //     rejectUnauthorized = false,
        //     perMessageDeflate = false, // 不压缩
        // });

        // 断线测试
        // setInterval(() => {
        //     if (this.socket && this.socket.readyState == WebSocket.OPEN) {
        //         this.socket.close();
        //     }
        // }, 2000);

        this.socket.on("open", () =>
        {
            this.onopen();
        });

        this.socket.on("close", (CloseCode code, string reason) =>
        {
            this.onclose(code, reason);
        });

        this.socket.on("error", (Error err) =>
        {
            this.server.baseScript.error("server-connect on error...." + err);
        });
    }
    public void onopen()
    {
        this.initMessageListener();
        if (this.onConnect != null)
        {
            this.onConnect(this);
        }
    }

    private int reconnectTimer = -1;
    public void onclose(CloseCode code, string reason)
    {
        this.server.baseScript.error("server-connect on close, code %d, reason %s, reconnect after 3 seconds", code, reason);
        if (this.onDisconnect != null)
        {
            this.onDisconnect(this);
        }
        // this.removeMessageListner();
        this.socket.removeAllListeners();
        this.socket = null;

        if (this.reconnectTimer != -1)
        {
            this.server.baseScript.clearTimer(this.reconnectTimer);
            this.reconnectTimer = -1;
        }

        this.reconnectTimer = this.server.baseScript.setTimer(() =>
        {
            this.open();
        }, this.reconnectInterval);
    }
}

// 文档
// https://github.com/websockets/ws
public class NetworkProtocolWS : NetworkProtocolBase, INetworkProtocol, IScript
{
    public Server server { get; set; }
    public string urlForServer(string host, int port)
    {
        var url = "ws://" + host + ":" + port;
        url += "?sign=" + ServerConst.SERVER_SIGN;
        url += "&purpose=" + this.server.purpose;
        return url;
    }
    public string getSocketId(WebSocketWrap wrap)
    {
        var s = wrap;
        if (s[MagicValue.SOCKET_ID] == undefined)
        {
            s[MagicValue.SOCKET_ID] = this.server.baseData.socketId++;
        }
        return s[MagicValue.SOCKET_ID];
    }
    public void closeSocket(WebSocketWrap wrap)
    {
        wrap.closeImmediately();
    }
    public WebSocketWrapC connect(string url, Action<WebSocketWrap> onConnect, Action<WebSocketWrap> onDisconnect)
    {
        var wrap = new WebSocketWrapC();
        wrap.server = this.server;
        wrap.url = url;
        wrap.onConnect = onConnect;
        wrap.onDisconnect = onDisconnect;
        wrap.open();
        return wrap;
    }
    public void send(WebSocketWrap wrap, MsgType type, object msg, Action<MyResponse> cb)
    {
        if (!this.isConnected(wrap))
        {
            if (cb != null)
            {
                cb(new MyResponse(ECode.NotConnected, null));
            }
            return;
        }

        var _package = msg;

        _package[MagicValue.MSG_TYPE] = type;

        var seq = this.server.baseData.msgSeq++;
        _package[MagicValue.MSG_SEQ] = seq;

        var str = this.server.JSON.stringify(_package);
        wrap.socket.send(str, (Error err) => {
            if (cb == null)
            {
                // this.server.logger.error(MsgType[type] + ": send error " + err);
            }
            else
            {
                if (err != null)
                {
                    this.server.baseScript.error(type.ToString() + ": send error " + err);
                    cb(new MyResponse(ECode.WebSocketError, err));
                }
                else
                {
                    this.server.baseData.pendingRequests[seq] = cb;
                }
            }
        });
    }
    public bool isConnected(WebSocketWrap wrap)
    {
        if (wrap == null || wrap.socket == null)
        {
            return false;
        }
        return wrap.socket.readyState == WebSocket.OPEN;
    }

    public void listen(int port, Func<bool> acceptClient, Action<WebSocketWrap, bool> onConnect, Action<WebSocketWrap, bool> onDisconnect)
    {
        var httpServer = http.createServer();
        var wsServer = new WebSocket.Server(new WebSocketOptions
        {
            // server: httpServer,
            noServer = true,
            perMessageDeflate = false,
        });
        wsServer.on("connection", (WebSocket socket, IncomingMessage request, bool isServer) =>
        {
            if (isServer != true && isServer != false)
            {
                this.server.baseScript.error("isServer not true not false, its " + isServer);
            }
            // var isServer = (request.headers.origin == ServerConst.SERVER_SIGN);
            var wrap = new WebSocketWrapS();
            wrap.server = this.server;
            wrap.socket = socket;

            wrap.socket.on("close", (CloseCode code, string reason) =>
            {
                this.server.logger.debug("server-listen on close, code %d, reason %s, isServer? %s", code, reason, (isServer ? "yes" : "no"));
                onDisconnect(wrap, isServer);
                wrap.socket.removeAllListeners();
            });
            wrap.socket.on("closeImmediately", () =>
            {
                this.server.logger.debug("server-listen on closeImmediately, isServer? %s", (isServer ? "yes" : "no"));
                onDisconnect(wrap, isServer);
                wrap.socket.removeAllListeners();
            });

            wrap.socket.on("error", (Error err) =>
            {
                this.server.logger.debug("server-listener on error error %s, isServer? %s....", (err != null ? err.toString() : "null"), (isServer ? "yes" : "no"));
            });

            wrap.initMessageListener();

            onConnect(wrap, isServer);

            // start ping
            wrap.setPing();
        });
        httpServer.on("upgrade", (string request, string netSocket, object head) =>
        {
            var fromServer = false;
            var query = url.parse(request.url || "", true).query;
            var sign = query["sign"];
            if (sign == ServerConst.SERVER_SIGN)
            {
                var purpose = query["purpose"];
                if (purpose != Purpose[this.server.purpose])
                {
                    this.server.baseScript.error("connect from different purpose " + purpose);
                    netSocket.write("HTTP/1.1 401 Unauthorized\r\n\r\n");
                    netSocket.destroy();
                    return;
                }
                fromServer = true;
            }
            else if (request.url.includes(ServerConst.CLIENT_SIGN))
            {
                if (!acceptClient())
                {
                    this.server.logger.info("client - server not ready!");
                    // next(new Error("client - server not ready!"));

                    netSocket.write("HTTP/1.1 503 server not ready\r\n\r\n");
                    netSocket.destroy();
                    return;
                }
                fromServer = false;
            }
            else
            {
                netSocket.write("HTTP/1.1 401 Unauthorized\r\n\r\n");
                netSocket.destroy();
                return;
            }

            wsServer.handleUpgrade(request, netSocket, head, (_WebSocket socket) =>
            {
                wsServer.emit("connection", _socket, request, fromServer);
            });
        });
        httpServer.listen(port, () =>
        {
            this.server.logger.info("listening on " + port);
        });
        // wsServer.on("listening", () => {
        //     this.server.logger.info();
        // });
    }
    // removeAllListeners(WebSocketWrap wrap): void {
    //     if (wrap.socket != null) {
    //         wrap.socket.removeAllListeners();
    //     }
    // }
    public void removeCustomMessageListener(WebSocketWrap wrap)
    {
        if (wrap.socket != null)
        {
            delete(wrap.socket as any)[MagicValue.SOCKET_CUS_MSG_FUN];
        }
    }
    public void setCustomMessageListener(WebSocketWrap wrap, Action<MsgType, object, Action<MyResponse>> fun)
    {
        if (wrap.socket != null)
        {
            wrap.socket[MagicValue.SOCKET_CUS_MSG_FUN] = fun;
        }
    }
}
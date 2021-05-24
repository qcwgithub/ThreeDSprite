using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class NetworkScript
{
    public Server server { get; set; }
    public string urlForServer(string host, int port)
    {
        var url = "ws://" + host + ":" + port;
        url += "?sign=" + ServerConst.SERVER_SIGN;
        url += "&purpose=" + this.server.purpose;
        return url;
    }
    public int getSocketId(object socket)
    {
        return ((MyWebSocket)socket).mySocketId;
    }
    public void closeSocket(object socket)
    {
        ((MyWebSocket)socket).closeImmediately();
    }

    public void bindPlayerAndSocket(PMPlayerInfo player, object socket, int clientTimestamp)
    {
        player.socket = socket;
        var ws = (MyWebSocket)socket;
        ws.Player = player;
        ws.clientTimestamp = clientTimestamp;
    }
    public void unbindPlayerAndSocket(PMPlayerInfo player, object socket)
    {
        player.socket = null;
        var wrap = socket as MyWebSocket;
        wrap.Player = null;
        wrap.clientTimestamp = 0;
    }

    public int getSocketClientTimestamp(object socket)
    {
        var wrap = socket as MyWebSocket;
        return wrap.clientTimestamp;
    }

    public PMPlayerInfo getPlayer(object socket)
    {
        return (PMPlayerInfo)((MyWebSocket)socket).Player;
    }

    public async Task<object> connectAsync(string url, Action<object, bool> onConnect, Action<object, bool> onDisconnect)
    {
        var ws = new MyWebSocketC();
        ws.mySocketId = this.server.baseData.socketId++;
        ws.server = this.server;
        ws.onConnect = onConnect;
        ws.onDisconnect = onDisconnect;
        ws.url = url;
        await ws.connectAsync();
        return ws;
    }

    public void send(object socket, MsgType type, string msg, Action<ECode, string> cb)
    {
        var ws = (MyWebSocket)socket;
        if (!this.isConnected(socket))
        {
            if (cb != null)
            {
                cb(ECode.NotConnected, null);
            }
            return;
        }
        ws.send(type, msg, cb);
    }

    public bool isConnected(object socket)
    {
        var ws = socket as MyWebSocket;
        return ws.IsConnected();
    }

    public async void listenAsync(int port, Func<bool> acceptClient,
        Action<object, bool> onConnect,
        Action<object, bool> onDisconnect)
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://*:" + port + "/");
        httpListener.Start();
        this.server.logger.info("listening on " + port);

        while (true)
        {
            HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();
            if (!httpListenerContext.Request.IsWebSocketRequest)
            {
                httpListenerContext.Response.StatusCode = 401;
                httpListenerContext.Response.Close();
                continue;
            }

            bool fromServer = false;

            string sign = httpListenerContext.Request.QueryString.Get("sign");
            if (sign == ServerConst.SERVER_SIGN)
            {
                string purpose = httpListenerContext.Request.QueryString.Get("purpose");
                if (purpose != this.server.purpose.ToString())
                {
                    this.server.baseScript.error("connect from different purpose " + purpose);
                    httpListenerContext.Response.StatusCode = 401;
                    httpListenerContext.Response.Close();
                    continue;
                }
                fromServer = true;
            }
            else if (httpListenerContext.Request.Url.ToString().Contains(ServerConst.CLIENT_SIGN))
            {
                if (!acceptClient())
                {
                    this.server.logger.info("client - server not ready!");
                    // next(new Error('client - server not ready!'));

                    httpListenerContext.Response.StatusCode = 503;
                    httpListenerContext.Response.Close();
                    return;
                }
                fromServer = false;
            }

            HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

            var ws = new MyWebSocketS();
            ws.mySocketId = this.server.baseData.socketId++;
            ws.server = this.server;
            ws.socket = webSocketContext.WebSocket;
            ws.onConnect = onConnect;
            ws.onDisconnect = onDisconnect;
            onConnect(ws, fromServer);

            // start ping
            ws.setPing();
        }
    }
    // removeAllListeners(WebSocketWrap wrap): void {
    //     if (wrap.socket != null) {
    //         wrap.socket.removeAllListeners();
    //     }
    // }
    public void removeCustomMessageListener(object socket)
    {
        var ws = (MyWebSocket)socket;
        ws.MessageListener = null;
    }
    public void setCustomMessageListener(object socket, Action<MsgType, string, Action<ECode, string>> fun)
    {
        var ws = (MyWebSocket)socket;
        ws.MessageListener = fun;
    }
}
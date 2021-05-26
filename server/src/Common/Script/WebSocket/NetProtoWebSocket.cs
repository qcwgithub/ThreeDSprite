using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

public class WebSocketData
{
    public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
    public int msgSeq = 1;
    public int socketId = 90000;
    public object listenerObject;
    public Dictionary<int, object> connectSockets = new Dictionary<int, object>();
}

public class NetProtoWebSocket : INetProto, IScript
{
    public Server server { get; set; }
    public WebSocketData data { get { return this.server.baseData.webSocketData; } }
    public string urlForServer(string host, int port)
    {
        var url = "ws://" + host + ":" + port;
        url += "?sign=" + ServerConst.SERVER_SIGN;
        url += "&purpose=" + this.server.purpose;
        return url;
    }

    // 连接其他服务器
    public async Task<ISocket> connectAsync(string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
    {
        var ws = new MyWebSocketC(this.data.socketId++, this.server, url, onConnect, onDisconnect);
        await ws.startAsync();
        return ws;
    }

    public async void listen(int port, Func<bool> acceptClient,
        Action<ISocket, bool> onConnect,
        Action<ISocket, bool> onDisconnect)
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://*:" + port + "/");
        httpListener.Start();
        this.server.logger.info("listening on " + port);

        while (true)
        {
            HttpListenerContext context = await httpListener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            bool fromServer = false;

            string sign = request.QueryString.Get("sign");
            if (sign == ServerConst.SERVER_SIGN)
            {
                string purpose = request.QueryString.Get("purpose");
                if (purpose != this.server.purpose.ToString())
                {
                    this.server.baseScript.error("connect from different purpose " + purpose);
                    response.StatusCode = 401;
                    response.Close();
                    continue;
                }
                fromServer = true;
            }
            else if (request.Url.ToString().Contains(ServerConst.CLIENT_SIGN))
            {
                if (!acceptClient())
                {
                    this.server.logger.info("client - server not ready!");
                    // next(new Error('client - server not ready!'));

                    response.StatusCode = 503;
                    response.Close();
                    return;
                }
                fromServer = false;
            }

            if (!request.IsWebSocketRequest)
            {
                this.server.logger.error("!IsWebSocketRequest, isServer ? " + fromServer);
                response.StatusCode = 401;
                response.Close();
                continue;
            }

            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);

            var ws = new MyWebSocketS(this.data.socketId++, server, webSocketContext.WebSocket, fromServer, onConnect, onDisconnect);
        }
    }
}
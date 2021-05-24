using System;
using System.Net.WebSockets;

public class MyWebSocketS : MyWebSocket
{
    public Action<object, bool> onConnect = null;
    public Action<object, bool> onDisconnect = null;
    public bool isConnectFromServer = false;
    protected override void CallOnConnect()
    {
        if (this.onConnect != null)
        {
            this.onConnect(this, this.isConnectFromServer);
        }
    }
    protected override void CallOnDisconnect()
    {
        if (this.onDisconnect != null)
        {
            this.onDisconnect(this, this.isConnectFromServer);
        }
    }

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
            if (this.socket.State == WebSocketState.Closed)
            {
                return;
            }
            this.server.logger.debug("ping pong timeout, close!! TODO");
            // this.closeImmediately();
           // this.socket.CloseAsync();
        }, timeout);
    }

    public void setPing()
    {
        // this.server.logger.info("setPing " + this.pingInterval);
        this.server.baseScript.clearTimer(this.pingTimer);
        this.pingTimer = this.server.baseScript.setTimer(() =>
        {
            // this.server.logger.info("send ping...");
            this.send("ping");
            this.onHearbeat(this.pingTimeout);
        }, this.pingInterval);
    }

    // override
    protected override void onMsg(string data)
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
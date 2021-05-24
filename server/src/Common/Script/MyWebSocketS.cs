public class MyWebSocketS : MyWebSocket
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
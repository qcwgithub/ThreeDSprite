using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

// 因为需要处理重连，只好包装一下
// only for connect
public class MyWebSocketC : MyWebSocket
{
    public string url = null;
    public int reconnectInterval = 3000;
    public async Task connectAsync()
    {
        if (this.reconnectTimer != -1)
        {
            this.server.baseScript.clearTimer(this.reconnectTimer);
            this.reconnectTimer = -1;
        }

        var clientSocket = new ClientWebSocket();
        this.socket = clientSocket;

        await clientSocket.ConnectAsync(new Uri(this.url), this.cancellationTaskSource.Token);
        
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
            this.connectAsync();
        }, this.reconnectInterval);
    }
}

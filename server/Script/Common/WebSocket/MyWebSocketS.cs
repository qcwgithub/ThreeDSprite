using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Script
{
    // 收到连接后（客户端或服务器）创建此对象
    public class MyWebSocketS : MyWebSocket
    {
        public bool isConnectFromServer { get; protected set; }
        public Action<ISocket, bool> onConnect { get; protected set; }
        public Action<ISocket, bool> onDisconnect { get; protected set; }

        public MyWebSocketS(int socketId, Server server,
            WebSocket socket, bool isConnectFromServer, Action<ISocket, bool> onConnect, Action<ISocket, bool> onDisconnect)
            : base(socketId, server)
        {
            this.socket = socket;
            this.isConnectFromServer = isConnectFromServer;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
        }

        protected void CallOnConnect()
        {
            if (this.onConnect != null)
            {
                this.onConnect(this, this.isConnectFromServer);
            }
        }

        protected void CallOnDisconnect()
        {
            if (this.onDisconnect != null)
            {
                this.onDisconnect(this, this.isConnectFromServer);
            }
        }

        protected override void doOnDisconnect()
        {
            this.CallOnDisconnect();

        }

        public void Start()
        {
            this.CallOnConnect();
        }
    }
}
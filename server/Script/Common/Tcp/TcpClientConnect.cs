using System;
using System.Threading.Tasks;

namespace Script
{
    // 用于服务器连接服务器
    public class TcpClientConnect : TcpClient
    {
        string url;
        Action<ISocket> onConnect;
        Action<ISocket> onDisconnect;

        public TcpClientConnect(int socketId, Server server, string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
            : base(socketId, server)
        {
            this.url = url;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;

            int index = this.url.LastIndexOf(':');
            string host = this.url.Substring(0, index);
            string p = this.url.Substring(index + 1);
            int port = int.Parse(p);

            _initConnectSocket(host, port);
        }

        protected override void onDisconnectComplete()
        {
            base.onDisconnectComplete();
            Console.WriteLine("Server disconnect");

            if (this.onDisconnect != null)
                this.onDisconnect(this);

            this.connectUntilSuccess();
        }

        public override async Task start()
        {
            await this.connectUntilSuccess();
        }

        private bool connecting = false;
        protected async Task connectUntilSuccess()
        {
            if (this.connecting || this.connected)
                return;

            while (true)
            {
                this.connecting = true;
                _connectAsync();

                while (this.connecting)
                    await Task.Delay(10);

                if (this.connected)
                    break;
            }
        }

        protected override void onConnectComplete()
        {
            this.connecting = false;

            this.connected = true;
            this.startRecv();
            this.startSend();

            if (this.onConnect != null)
                this.onConnect(this);
        }
    }
}
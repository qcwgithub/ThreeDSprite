using System;
using System.Threading.Tasks;

namespace Data
{
    // 用于服务器连接服务器
    public class TcpClientConnectData : TcpClientData
    {
        string url;

        public TcpClientConnectData(int socketId, ServerBaseData serverData, string url)
            : base(socketId, serverData)
        {
            this.url = url;

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

            this.serverData.scriptProxy.onDisconnect(true, this);
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
            
            this.serverData.scriptProxy.onConnect(true, this);
        }
    }
}
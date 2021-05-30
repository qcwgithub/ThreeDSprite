using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Data
{
    public class TcpClientAcceptData : TcpClientData
    {
        public bool isConnectFromServer { get; protected set; }

        public TcpClientAcceptData(int socketId, ServerBaseData serverData,
            Socket socket, bool isConnectFromServer)
            : base(socketId, serverData)
        {
            _initAcceptSocket(socket);
            this.isConnectFromServer = isConnectFromServer;
            this.connected = true;
        }
        public override bool isMessageFromServer { get { return this.isConnectFromServer; } }

        protected override void onConnectComplete()
        {
            throw new NotImplementedException();
        }

        protected override void onDisconnectComplete()
        {
            base.onDisconnectComplete();
            this.serverData.scriptProxy.acceptorOnDisconnect(this, this.isConnectFromServer);
        }

        public override Task start()
        {
            this.startRecv();
            this.startSend();

            this.serverData.scriptProxy.acceptorOnConnect(this, this.isConnectFromServer);
            return Task.CompletedTask;
        }
    }
}
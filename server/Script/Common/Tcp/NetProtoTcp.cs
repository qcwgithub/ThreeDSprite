using System;
using System.Net.Sockets;
using Data;

namespace Script
{
    public class NetProtoTcp : IServerScript<Server>
    {
        public Server server { get; set; }
        public TcpData tcpData
        {
            get
            {
                return this.server.baseData.tcpData;
            }
        }

        public string urlForServer(string host, int port)
        {
            var url = //"http://" + 
            host + ":" + port;
            //url += "?sign=" + ServerConst.SERVER_SIGN;
            //url += "&purpose=" + this.server.purpose;
            return url;
        }

        public void onListenSocketComplete(SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.onAcceptComplete(e);
                    break;
                default:
                    throw new Exception($"socket accept error: {e.LastOperation}");
            }
        }

        private void acceptAsync(SocketAsyncEventArgs e)
        {
            this.server.baseData.tcpData.acceptAsync(e);
        }

        public void onAcceptComplete(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                //Log.Error($"accept error {innArgs.SocketError}");
                this.acceptAsync(e);
                return;
            }

            bool isServer = true; // TODO
            this.tcpData.acceptData = new TcpClientAcceptData(this.tcpData.socketId++, this.tcpData, e.AcceptSocket, isServer, this.tcpData.onConnect, this.tcpData.onDisconnect);
            this.tcpData.acceptData.start();

            // continue accept
            this.acceptAsync(e);
        }
    }
}
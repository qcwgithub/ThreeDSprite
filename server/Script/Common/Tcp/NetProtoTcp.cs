using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class NetProtoTcp : INetProto, IServerScript<Server>
    {
        public Server server { get; set; }
        public TcpData data { get { return this.server.baseData.tcpData; } }

        public string urlForServer(string host, int port)
        {
            var url = //"http://" + 
            host + ":" + port;
            //url += "?sign=" + ServerConst.SERVER_SIGN;
            //url += "&purpose=" + this.server.purpose;
            return url;
        }

        public async Task<ISocket> connectAsync(string url, Action<ISocket> onConnect, Action<ISocket> onDisconnect)
        {
            var tcp = new TcpClientConnect(this.data.socketId++, this.server, url, onConnect, onDisconnect);
            await tcp.start();
            return tcp;
        }

        private void onComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ET.ThreadSynchronizationContext.Instance.Post(this.onAcceptComplete, e);
                    break;
                default:
                    throw new Exception($"socket accept error: {e.LastOperation}");
            }
        }
        private void acceptAsync(SocketAsyncEventArgs e, Socket socket)
        {
            e.AcceptSocket = null;
            bool completed = !socket.AcceptAsync(e);
            if (completed)
            {
                this.onAcceptComplete(e);
            }
        }

        private void onAcceptComplete(object _e)
        {
            var e = (SocketAsyncEventArgs)_e;
            if (e.SocketError != SocketError.Success)
            {
                //Log.Error($"accept error {innArgs.SocketError}");
                this.acceptAsync(e, this.data.socket);
                return;
            }

            bool isServer = true; // TODO
            var s = new TcpClientAccept(this.data.socketId++, this.server, e.AcceptSocket, isServer, this.data.onConnect, this.data.onDisconnect);
            s.start();

            // continue accept
            this.acceptAsync(e, this.data.socket);
        }

        public void listen(int port, Func<bool> acceptClient, Action<ISocket, bool> onConnect, Action<ISocket, bool> onDisconnect)
        {
            var socket = this.data.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var e = this.data.e = new SocketAsyncEventArgs();

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(1000);

            e.Completed += this.onComplete;

            this.data.onConnect = onConnect;
            this.data.onDisconnect = onDisconnect;

            this.acceptAsync(e, socket);
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;

namespace Data
{
    public partial class TcpListenerData
    {
        public void listen(int port)
        {
            var address = IPAddress.IPv6Any;
            this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocketArg = new SocketAsyncEventArgs();
            this.listenSocketArg.Completed += this._onComplete;

            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            // object ipv6Only = this.socket.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only);
            this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            this.socket.Bind(new IPEndPoint(address, port));
            this.socket.Listen(1000);
        }

        public void onComplete(object _e)
        {
            var e = (SocketAsyncEventArgs)_e;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.onAcceptComplete();
                    break;
                default:
                    throw new Exception($"socket accept error: {e.LastOperation}");
            }
        }

        void onAcceptComplete()
        {
            this.accepting = false;

            var callback = this.serverData.tcpListenerCallback;
            callback.onAcceptComplete(this, this.listenSocketArg);
        }

        public void accept()
        {
            this.accepting = true;
            this.listenSocketArg.AcceptSocket = null;
            bool completed = !this.socket.AcceptAsync(this.listenSocketArg);
            if (completed)
            {
                this.onAcceptComplete();
            }
        }
    }
}
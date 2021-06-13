using System;
using Data;
using System.Net;
using System.Net.Sockets;

namespace Script
{
    public static class TcpListenerExtension
    {
        public static void listen(this TcpListenerData @this, int port)
        {
            var address = IPAddress.IPv6Any;
            @this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            @this.listenSocketArg = new SocketAsyncEventArgs();
            @this.listenSocketArg.Completed += @this._onComplete;

            @this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // @this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            // object ipv6Only = @this.socket.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only);
            @this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            @this.socket.Bind(new IPEndPoint(address, port));
            @this.socket.Listen(1000);
        }

        public static void onComplete(this TcpListenerData @this, object _e)
        {
            var e = (SocketAsyncEventArgs)_e;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    @this.onAcceptComplete();
                    break;
                default:
                    throw new Exception($"socket accept error: {e.LastOperation}");
            }
        }

        static void onAcceptComplete(this TcpListenerData @this)
        {
            @this.accepting = false;

            var scriptProxy = @this.serverData.scriptProxy;
            if (scriptProxy == null)
            {
                @this.serverData.logger.Error("onAcceptComplete: scriptProxy == null");
                return;
            }
            if (scriptProxy.onAcceptComplete == null)
            {
                @this.serverData.logger.Error("scriptProxy.onAcceptComplete == null");
                return;
            }

            scriptProxy.onAcceptComplete(@this, @this.listenSocketArg);
        }

        public static void accept(this TcpListenerData @this)
        {
            @this.accepting = true;
            @this.listenSocketArg.AcceptSocket = null;
            bool completed = !@this.socket.AcceptAsync(@this.listenSocketArg);
            if (completed)
            {
                @this.onAcceptComplete();
            }
        }
    }
}
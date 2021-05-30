using System;
using System.Net.Sockets;
using Data;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace Script
{
    public class NetProtoTcp : IServerScript<Server>, IScriptProxy
    {
        public Server server { get; set; }
        public TcpData tcpData
        {
            get
            {
                return this.server.baseData.tcpData;
            }
        }

        /////////////////////// connect ///////////////////////////

        public string urlForServer(string host, int port)
        {
            var url = //"http://" + 
            host + ":" + port;
            //url += "?sign=" + ServerConst.SERVER_SIGN;
            //url += "&purpose=" + this.server.purpose;
            return url;
        }

        public async Task<ISocket> connectAsync(string url)
        {
            var tcpData = this.tcpData;

            tcpData.connectorData = new TcpClientConnectData(this.tcpData.socketId++, this.server.baseData, url);
            await tcpData.connectorData.start();
            return tcpData.connectorData;
        }

        string msgOnConnect = null;
        public void connectorOnConnect(TcpClientData tcpClientData)
        {
            if (this.msgOnConnect == null)
            {
                this.msgOnConnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
            }
            this.server.dispatcher.dispatch(tcpClientData, MsgType.OnConnect, this.msgOnConnect, null);
        }

        string msgOnDisconnect = null;
        public void connectorOnDisconnect(TcpClientData tcpClientData)
        {
            if (this.msgOnDisconnect == null)
            {
                this.msgOnDisconnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
            }
            this.server.dispatcher.dispatch(tcpClientData, MsgType.OnDisconnect, this.msgOnDisconnect, null);
        }

        /////////////////////// accept ///////////////////////////

        public void listen(int port, Func<bool> acceptClient)
        {
            var tcpData = this.tcpData;

            var socket = tcpData.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var e = tcpData.listenSocketArg = new SocketAsyncEventArgs();
            e.Completed += tcpData._eCompleted_multiThreaded;

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(1000);

            this.acceptAsync(e);
        }

        public void onListenerSocketComplete(SocketAsyncEventArgs e)
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
            e.AcceptSocket = null;
            bool completed = !this.tcpData.socket.AcceptAsync(e);
            if (completed)
            {
                // this.onAcceptComplete(e); // moved to script
                this.onAcceptComplete(e);
            }
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
            this.tcpData.acceptorData = new TcpClientAcceptData(this.tcpData.socketId++, this.server.baseData, e.AcceptSocket, isServer);
            this.tcpData.acceptorData.start();

            // continue accept
            this.acceptAsync(e);
        }

        string msgOnConnect_true = null;
        string msgOnConnect_false = null;
        public void acceptorOnConnect(TcpClientData tcpClientData, bool isServer)
        {
            if (this.msgOnConnect_true == null)
            {
                this.msgOnConnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
                this.msgOnConnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
            }

            this.server.dispatcher.dispatch(tcpClientData, MsgType.OnConnect, isServer ? msgOnConnect_true : msgOnConnect_false, null);
        }

        string msgOnDisconnect_true = null;
        string msgOnDisconnect_false = null;
        public void acceptorOnDisconnect(TcpClientData tcpClientData, bool isServer)
        {
            if (this.msgOnDisconnect_true == null)
            {
                this.msgOnDisconnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
                this.msgOnDisconnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
            }

            this.server.dispatcher.dispatch(tcpClientData, MsgType.OnDisconnect, isServer ? msgOnDisconnect_true : msgOnDisconnect_false, null);
        }
    }
}
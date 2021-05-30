using System;
using System.Net.Sockets;
using Data;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace Script
{
    public class TcpListenerScript : IServerScript<Server>
    {
        public Server server { get; set; }
        public TcpListenerData @this
        {
            get
            {
                return this.server.baseData.tcpListener;
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

        public async Task<TcpClientData> connectAsync(string url)
        {
            @this.connectorData = new TcpClientData();
            this.server.tcpClientScript.connectorConstructor(@this.connectorData, url, @this.socketId++, this.server.baseData);
            await this.server.tcpClientScript.start(@this.connectorData);
            return @this.connectorData;
        }

        string msgOnConnect = null;
        string msgOnDisconnect = null;

        string msgOnConnect_true = null;
        string msgOnConnect_false = null;
        string msgOnDisconnect_true = null;
        string msgOnDisconnect_false = null;

        void initMsgConnect()
        {
            if (msgOnConnect == null)
            {
                this.msgOnConnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
                this.msgOnDisconnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
                this.msgOnConnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
                this.msgOnConnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
                this.msgOnDisconnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
                this.msgOnDisconnect_false = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
            }
        }
        public void onConnect(TcpClientData tcpClient)
        {
            this.initMsgConnect();
            if (tcpClient.isConnector)
            {
                this.server.dispatcher.dispatch(tcpClient, MsgType.OnConnect, this.msgOnConnect, null);
            }
            else
            {
                this.server.dispatcher.dispatch(tcpClient, MsgType.OnConnect, tcpClient.connectedFromServer ? msgOnConnect_true : msgOnConnect_false, null);
            }
        }

        public void onDisconnect(TcpClientData tcpClient)
        {
            this.initMsgConnect();
            if (tcpClient.isConnector)
            {
                this.server.dispatcher.dispatch(tcpClient, MsgType.OnDisconnect, this.msgOnDisconnect, null);
            }
            else
            {
                this.server.dispatcher.dispatch(tcpClient, MsgType.OnDisconnect, tcpClient.connectedFromServer ? msgOnDisconnect_true : msgOnDisconnect_false, null);
            }
        }

        /////////////////////// accept ///////////////////////////

        public void listen(int port, Func<bool> acceptClient)
        {
            var socket = @this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var e = @this.listenSocketArg = new SocketAsyncEventArgs();
            e.Completed += @this._eCompleted_multiThreaded;

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(1000);

            this.acceptAsync(e);
        }

        public void onTcpListenerComplete(SocketAsyncEventArgs e)
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
            bool completed = !@this.socket.AcceptAsync(e);
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
            @this.acceptorData = new TcpClientData();
            this.server.tcpClientScript.acceptorConstructor(@this.acceptorData, e.AcceptSocket, @this.socketId++, this.server.baseData, isServer);
            this.server.tcpClientScript.start(@this.acceptorData);

            // continue accept
            this.acceptAsync(e);
        }
    }
}
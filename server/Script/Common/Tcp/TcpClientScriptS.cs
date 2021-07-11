using System;
using Data;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Script
{
    public class TcpClientScriptS : TcpClientScript, IServerScript<Server>
    {
        public Server server { get; set; }

        protected override void logError(TcpClientData @this, string str)
        {
            this.server.logger.Error(str);
        }
        protected override void logInfo(TcpClientData @this, string str)
        {
            this.server.logger.Info(str);
        }
        public override TcpClientScriptProxy tcpClientScriptProxy
        {
            get
            {
                return this.server.data.tcpClientScriptProxy;
            }
        }
        protected override IMessagePacker messagePacker
        {
            get
            {
                return this.server.messagePacker;
            }
        }

        protected override int nextSocketId
        {
            get
            {
                return this.server.data.socketId++;
            }
        }

        protected override int nextMsgSeq
        {
            get
            {
                return this.server.data.msgSeq++;
            }
        }

        public TcpClientData acceptorConstructor(TcpClientData @this, Socket socket, bool connectedFromClient)
        {
            @this.isConnector = false;

            @this._socket = socket;
            // @this._cancellationTaskSource = new CancellationTokenSource();
            // @this._cancellationToken = @this._cancellationTaskSource.Token;
            @this._innArgs = new SocketAsyncEventArgs();
            @this._outArgs = new SocketAsyncEventArgs();
            @this._innArgs.Completed += @this._onComplete;
            @this._outArgs.Completed += @this._onComplete;

            @this.proxyProvider = this.server.data;
            @this.socketId = this.nextSocketId;
            @this.oppositeIsClient = connectedFromClient;

            @this.connecting = false;
            @this.connected = true;
            @this.sending = false;
            @this.closed = false;
            return @this;
        }

        #region basic access
        public bool isServerConnected(int serverId)
        {
            TcpClientData socket;
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !this.isConnected(socket))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region bind player

        public void bindPlayer(TcpClientData @this, PMPlayerInfo player, int clientTimestamp)
        {
            player.socket = @this;
            @this.Player = player;
            @this.clientTimestamp = clientTimestamp;
        }

        public void unbindPlayer(TcpClientData @this, PMPlayerInfo player)
        {
            player.socket = null;
            @this.Player = null;
            @this.clientTimestamp = 0;
        }

        public void bindPlayer(TcpClientData @this, BMPlayerInfo player, int clientTimestamp)
        {
            player.socket = @this;
            @this.Player = player;
            @this.clientTimestamp = clientTimestamp;
        }

        public void unbindPlayer(TcpClientData @this, BMPlayerInfo player)
        {
            player.socket = null;
            @this.Player = null;
            @this.clientTimestamp = 0;
        }

        public int getClientTimestamp(TcpClientData @this)
        {
            return @this.clientTimestamp;
        }

        public object getPlayer(TcpClientData @this)
        {
            return @this.Player == null ? null : @this.Player;
        }
        

        #endregion

        #region send
        public async Task<MyResponse> sendToServerAsync(int serverId, MsgType type, object msg)
        {
            TcpClientData socket;
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !this.isConnected(socket))
            {
                return ECode.NotConnected;
            }
            return await this.sendAsync(socket, type, msg);
        }

        public void sendToServer(int serverId, MsgType type, object msg, Action<ECode, object> cb)
        {
            TcpClientData socket;
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !this.isConnected(socket))
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }
            this.send(socket, type, msg, cb);
        }
        #endregion

        #region connect

        public override void onConnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            bool success = e.SocketError == SocketError.Success;
            if (success)
            {
                var msg = new MsgOnConnect
                {
                    isAcceptor = !@this.isConnector,
                    // isServer = @this.connectedFromServer,
                };
                this.tcpClientScriptProxy.dispatch(@this, MsgType.OnSocketConnect, msg, null);

                this.recv(@this);
                this.send(@this);
            }
            else
            {
                this.logError(@this, "TcpClientScriptS.onConnectComplete, e.SocketError = " + e.SocketError);
            }
        }
        #endregion

        #region disconnect

        public override void onDisconnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            // this.logError(@this, "TcpClientScriptS.onDisconnectComplete, e.SocketError = " + e.SocketError);
            // var msg = new MsgOnDisconnect
            // {
            //     isAcceptor = !@this.isConnector,
            //     // isServer = @this.connectedFromServer,
            // };
            // this.tcpClientScriptProxy.dispatch(@this, MsgType.OnDisconnect, msg, null);
            this.close(@this, "onDisconnectComplete");
        }

        public override void onCloseComplete(TcpClientData @this)
        {
            base.onCloseComplete(@this);
            // this.logError(@this, "TcpClientScriptS.onClose");
            var msg = new MsgOnClose
            {
                isAcceptor = !@this.isConnector,
                // isServer = @this.connectedFromServer,
            };
            this.tcpClientScriptProxy.dispatch(@this, MsgType.OnSocketClose, msg, null);
        }

        #endregion
    }
}
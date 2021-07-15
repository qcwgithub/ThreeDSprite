using System;
using Data;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Script
{
    public class TcpClientScriptS : IServerScript<Server>, ITcpClientCallback
    {
        public Server server { get; set; }

        /////////////////////////////////////////////////////////////////
        #region  ITcpClientCallback

        public void logError(TcpClientData data, string str)
        {
            this.server.logger.Error(str);
        }

        public void logInfo(TcpClientData data, string str)
        {
            this.server.logger.Info(str);
        }

        public IMessagePacker messagePacker
        {
            get
            {
                return this.server.messagePacker;
            }
        }

        public int nextSocketId
        {
            get
            {
                return this.server.data.socketId++;
            }
        }

        public int nextMsgSeq
        {
            get
            {
                return this.server.data.msgSeq++;
            }
        }

        public void dispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            this.server.rawDispatch(data, msgType, msg, reply);
        }

        public void onConnectComplete(TcpClientData data, SocketAsyncEventArgs e, SocketError socketError)
        {
            if (socketError == SocketError.Success)
            {
                var msg = new MsgOnConnect
                {
                    isAcceptor = !data.isConnector,
                    // isServer = @this.connectedFromServer,
                };
                this.server.rawDispatch(data, MsgType.OnSocketConnect, msg, null);
            }
        }

        public void onCloseComplete(TcpClientData data)
        {
            var msg = new MsgOnClose
            {
                isAcceptor = !data.isConnector,
                // isServer = @this.connectedFromServer,
            };
            this.server.rawDispatch(data, MsgType.OnSocketClose, msg, null);
        }

        #endregion
        /////////////////////////////////////////////////////////////////

        #region basic access
        public bool isServerConnected(int serverId)
        {
            TcpClientData socket;
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !socket.isConnected())
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
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !socket.isConnected())
            {
                return ECode.NotConnected;
            }
            return await socket.sendAsync(type, msg);
        }

        public void sendToServer(int serverId, MsgType type, object msg, Action<ECode, object> cb)
        {
            TcpClientData socket;
            if (!this.server.data.otherServerSockets.TryGetValue(serverId, out socket) || !socket.isConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, null);
                }
                return;
            }
            socket.send(type, msg, cb);
        }
        #endregion
    }
}
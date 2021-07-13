using System;
using Data;
using System.Net;
using System.Net.Sockets;

namespace Script
{
    public abstract class TcpClientScriptBasic
    {
        protected abstract void logError(TcpClientData @this, string str);
        protected abstract void logInfo(TcpClientData @this, string str);
        public abstract TcpClientScriptProxy tcpClientScriptProxy { get; }

        public void onSomethingComplete(TcpClientData @this, object _e)
        {
            try
            {
                var e = (SocketAsyncEventArgs)_e;
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        {
                            this.onConnectComplete(@this, e);
                        }
                        break;
                    case SocketAsyncOperation.Disconnect:
                        {
                            this.onDisconnectComplete(@this, e);
                        }
                        break;
                    case SocketAsyncOperation.Send:
                        {
                            this.onSendComplete(@this, e);
                        }
                        break;
                    case SocketAsyncOperation.Receive:
                        {
                            this.onRecvComplete(@this, e);
                        }
                        break;
                    default:
                        this.logError(@this, "TcpClientScriptBase.onSomethingComplete default: " + e.LastOperation);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.logError(@this, "onSomethingComplete " + ex);
            }
        }

        #region connect

        public void connect(TcpClientData @this)
        {
            try
            {
                @this.connecting = true;
                @this._outArgs.RemoteEndPoint = @this.ipEndPointForConnector;
                bool completed = !@this._socket.ConnectAsync(@this._outArgs);
                if (completed)
                {
                    this.onConnectComplete(@this, @this._outArgs);
                }
            }
            catch (SocketException ex)
            {
                this.logError(@this, "connect exception" + ex);
            }
        }

        void onConnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            @this.connecting = false;
            e.RemoteEndPoint = null;
            if (e.SocketError == SocketError.Success)
            {
                @this.connected = true;
            }
            this.tcpClientScriptProxy.onConnectComplete(@this, e);
        }

        #endregion

        #region disconnect

        public void close(TcpClientData @this, string reason)
        {
            if (@this.closed)
            {
                // this.logError(@this, $"call close on socketId({@this.socketId}) with reason({reason}), but @this.closed is true!");
                return;
            }
            // this.logInfo(@this, $"call close on socketId({@this.socketId}) with reason({reason})");
            @this.closed = true;

            if (@this._socket.Connected)
            {
                @this._socket.Shutdown(SocketShutdown.Both);
            }
            @this._socket.Close();
            @this._socket = null;

            @this.connected = false;
            @this.connecting = false;
            @this.sending = false;

            @this._innArgs.Completed -= @this._onComplete;
            @this._innArgs.Dispose();
            @this._innArgs = null;

            @this._outArgs.Completed -= @this._onComplete;
            @this._outArgs.Dispose();
            @this._outArgs = null;

            @this.proxyProvider = null;

            this.tcpClientScriptProxy.onCloseComplete(@this);
        }

        void onDisconnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            @this.connected = false;
            this.tcpClientScriptProxy.onDisconnectComplete(@this, e);
        }

        #endregion

        #region send

        public void send(TcpClientData @this)
        {
            if (!@this.connected || @this.sending || @this.sendList.Count == 0)
            {
                return;
            }

            @this.sending = true;

            var bytes = @this.sendList[0];
            @this.sendList.RemoveAt(0);

            this.sendAsync(@this, bytes, 0, bytes.Length);
        }

        void sendAsync(TcpClientData @this, byte[] buffer, int offset, int count)
        {
            try
            {
                @this._outArgs.SetBuffer(buffer, 0, buffer.Length);
                bool completed = !@this._socket.SendAsync(@this._outArgs);
                if (completed)
                {
                    this.onSendComplete(@this, @this._outArgs);
                }
            }
            catch (Exception e)
            {
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
                this.close(@this, "sendAsync Exception " + e);
            }
        }

        void onSendComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            @this.sending = false;
            this.tcpClientScriptProxy.onSendComplete(@this, e);
        }

        #endregion

        #region recv

        public void recv(TcpClientData @this)
        {
            this.recvAsync(@this, @this.recvBuffer, @this.recvOffset, @this.recvBuffer.Length - @this.recvOffset);
        }

        void recvAsync(TcpClientData @this, byte[] buffer, int offset, int count)
        {
            try
            {
                @this._innArgs.SetBuffer(buffer, offset, buffer.Length - offset);
                bool completed = !@this._socket.ReceiveAsync(@this._innArgs);
                if (completed)
                {
                    this.onRecvComplete(@this, @this._innArgs);
                }
            }
            catch (Exception e)
            {
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}", e);
                this.close(@this, "recvAsync Exception " + e);
            }

        }

        void onRecvComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            this.tcpClientScriptProxy.onRecvComplete(@this, e);
        }

        #endregion
    }
}
using System;
using System.Net;
using System.Net.Sockets;

namespace Data
{
    public partial class TcpClientData
    {
        void onSomethingComplete(object _e)
        {
            if (this.closed)
            {
                return;
            }
            try
            {
                var e = (SocketAsyncEventArgs)_e;
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        {
                            this.onConnectComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Disconnect:
                        {
                            this.onDisconnectComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Send:
                        {
                            this.onSendComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Receive:
                        {
                            this.onRecvComplete(e);
                        }
                        break;
                    default:
                        this.callback.logError(this, "TcpClientScriptBase.onSomethingComplete default: " + e.LastOperation);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.callback.logError(this, "onSomethingComplete " + ex);
            }
        }

        #region connect

        public void connect()
        {
            try
            {
                this.connecting = true;
                this._outArgs.RemoteEndPoint = this.ipEndPointForConnector;
                bool completed = !this._socket.ConnectAsync(this._outArgs);
                if (completed)
                {
                    this.onConnectComplete(this._outArgs);
                }
            }
            catch (SocketException ex)
            {
                this.callback.logError(this, "connect exception" + ex);
            }
        }

        void onConnectComplete(SocketAsyncEventArgs e)
        {
            this.connecting = false;
            e.RemoteEndPoint = null;
            if (e.SocketError == SocketError.Success)
            {
                this.connected = true;
            }

            this.callback.onConnectComplete(this, e, e.SocketError);
            
            bool success = e.SocketError == SocketError.Success;
            if (success)
            {
                this.recv();
                this.send();
            }
            else
            {
                this.callback.logError(this, "TcpClientScriptS.onConnectComplete, e.SocketError = " + e.SocketError);
            }
        }

        #endregion

        #region disconnect

        public void close(string reason)
        {
            if (this.closed)
            {
                // this.logError(this, $"call close on socketId({this.socketId}) with reason({reason}), but this.closed is true!");
                return;
            }
            // this.logInfo(this, $"call close on socketId({this.socketId}) with reason({reason})");
            this.closed = true;

            if (this._socket.Connected)
            {
                this._socket.Shutdown(SocketShutdown.Both);
            }
            this._socket.Close();
            this._socket = null;

            this.connected = false;
            this.connecting = false;
            this.sending = false;

            this._innArgs.Completed -= this._onComplete;
            this._innArgs.Dispose();
            this._innArgs = null;

            this._outArgs.Completed -= this._onComplete;
            this._outArgs.Dispose();
            this._outArgs = null;


            this.onCloseComplete();
            this.callback = null;
        }

        void onDisconnectComplete(SocketAsyncEventArgs e)
        {
            this.connected = false;
            this.close("onDisconnectComplete");
        }

        #endregion

        #region send

        public void send()
        {
            if (!this.connected || this.sending || this.sendList.Count == 0)
            {
                return;
            }

            this.sending = true;

            var bytes = this.sendList[0];
            this.sendList.RemoveAt(0);

            this.sendAsync(bytes, 0, bytes.Length);
        }

        void sendAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._outArgs.SetBuffer(buffer, 0, buffer.Length);
                bool completed = !this._socket.SendAsync(this._outArgs);
                if (completed)
                {
                    this.onSendComplete(this._outArgs);
                }
            }
            catch (Exception e)
            {
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
                this.close("sendAsync Exception " + e);
            }
        }

        #endregion

        #region recv

        public void recv()
        {
            this.recvAsync(this.recvBuffer, this.recvOffset, this.recvBuffer.Length - this.recvOffset);
        }

        void recvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._innArgs.SetBuffer(buffer, offset, buffer.Length - offset);
                bool completed = !this._socket.ReceiveAsync(this._innArgs);
                if (completed)
                {
                    this.onRecvComplete(this._innArgs);
                }
            }
            catch (Exception e)
            {
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}", e);
                this.close("recvAsync Exception " + e);
            }

        }

        #endregion
    }
}
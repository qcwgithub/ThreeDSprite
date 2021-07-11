using System;
using Data;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class TcpClientScriptC : TcpClientScript, ITcpClientScriptProxyProvider
    {
        public static int s_msgSeq = 1;
        public static int s_socketId = 1000;

        TcpClientScriptProxy _tcpClientScriptProxy;
        TcpClientData _tcpClientData;
        private string ip;
        private int port;
        public event Action<TcpClientData, MsgType, object, Action<ECode, object>> onReceiveMessageFromServer;
        public TcpClientScriptC(string ip, int port)
        {
            _tcpClientData = new TcpClientData();
            this.connectorConstructor(_tcpClientData, ip, port, this);

            this._tcpClientScriptProxy = new TcpClientScriptProxy
            {
                onSomethingComplete = (tcpClient, e) => this.onSomethingComplete(tcpClient, e),
                onConnectComplete = (tcpClient, e) => this.onConnectComplete(tcpClient, e),
                onDisconnectComplete = (tcpClient, e) => this.onDisconnectComplete(tcpClient, e),
                onSendComplete = (tcpClient, e) => this.onSendComplete(tcpClient, e),
                onRecvComplete = (tcpClient, e) => this.onRecvComplete(tcpClient, e),
                onCloseComplete = (tcpClient) => this.onCloseComplete(tcpClient),

                dispatch = (tcpClient, msgType, msg, reply) =>
                {
                    // this.dispatcher.dispatch(tcpClient, msgType, msg, reply),

                    if (this.onReceiveMessageFromServer != null)
                    {
                        this.onReceiveMessageFromServer(tcpClient, msgType, msg, reply);
                    }
                    else
                    {
                        Debug.LogError("receieve message from server, no handler");
                    }
                }
            };
        }

        private Dictionary<int, Action<ECode, object>> _waitingResponses = new Dictionary<int, Action<ECode, object>>();

        public bool isConnected()
        {
            return this.isConnected(this._tcpClientData);
        }

        public void open()
        {
            this.connect(this._tcpClientData);
        }

        protected override void logError(TcpClientData @this, string str)
        {
            Debug.LogError(str);
        }
        protected override void logInfo(TcpClientData @this, string str)
        {
            Debug.Log(str);
        }
        public override TcpClientScriptProxy tcpClientScriptProxy
        {
            get
            {
                return _tcpClientScriptProxy;
            }
        }
        protected override IMessagePacker messagePacker
        {
            get
            {
                // return JsonMessagePackerC.Instance;
                return BinaryMessagePacker.Instance;
            }
        }

        protected override int nextSocketId
        {
            get
            {
                return s_socketId++;
            }
        }

        protected override int nextMsgSeq
        {
            get
            {
                return s_msgSeq++;
            }
        }

        #region connect

        public Action<bool, string> onConnect;
        public override void onConnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            //var msg = new MsgOnConnect { isListen = !@this.isConnector, isServer = @this.connectedFromServer };
            //this.tcpClientScriptProxy.dispatch(@this, MsgType.OnConnect, msg, null);
            // this.logError(@this, "TcpClientScriptC.onConnectComplete, e.SocketError = " + e.SocketError);
            bool success = e.SocketError == SocketError.Success;
            if (success)
            {
                this.recv(@this);
                this.send(@this);
            }

            if (onConnect != null)
            {
                onConnect(success, "SocketError." + e.SocketError);
            }
        }
        #endregion

        #region disconnect

        public Action<string> onClose;
        public override void onDisconnectComplete(TcpClientData @this, SocketAsyncEventArgs e)
        {
            //var msg = new MsgOnDisconnect { isListen = !@this.isConnector, isServer = @this.connectedFromServer };
            //this.tcpClientScriptProxy.dispatch(@this, MsgType.OnDisconnect, msg, null);
            // Debug.Log("TcpClientScriptC.onDisconnectComplete");
            // if (onDisconnect != null)
            // {
            //     onDisconnect("to do: message");
            // }
            this.close(@this, "onDisconnectComplete");
        }

        public override void onCloseComplete(TcpClientData @this)
        {
            base.onCloseComplete(@this);
            // this.logError(@this, "TcpClientScriptS.onClose");
            if (onClose != null)
            {
                onClose("to do: message");
            }
        }

        #region send
        public void send<T>(MsgType msgType, T msg, Action<ECode, object> cb, int timeoutMs)
        {
            this.send(this._tcpClientData, msgType, msg, cb);
        }
        #endregion

        public void cleanup()
        {
            this.onConnect = null;
            this.onClose = null;
            if (!this._tcpClientData.closed)
            {
                this.close(this._tcpClientData, "TcpClientScriptC.cleanup");
            }
            foreach (var kv in this._waitingResponses)
            {
                kv.Value(ECode.Timeout, null);
            }
            this._waitingResponses.Clear();
        }

        #endregion
    }
}
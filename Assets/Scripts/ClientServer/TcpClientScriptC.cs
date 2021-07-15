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
    public class TcpClientScriptC : ITcpClientCallback
    {
        TcpClientData tcpClientData;
        private string ip;
        private int port;
        public event Action<TcpClientData, MsgType, object, Action<ECode, object>> onReceiveMessageFromServer;
        public TcpClientScriptC(string ip, int port)
        {
            this.tcpClientData = new TcpClientData();
            this.tcpClientData.connectorInit(this, ip, port);
        }

        /////////////////////////////////////////////////////////////////
        #region  ITcpClientCallback

        public void logError(TcpClientData data, string str)
        {
            Debug.LogError(str);
        }

        public void logInfo(TcpClientData data, string str)
        {
            Debug.Log(str);
        }

        public void dispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            // this.dispatcher.dispatch(tcpClient, msgType, msg, reply),

            if (this.onReceiveMessageFromServer != null)
            {
                this.onReceiveMessageFromServer(data, msgType, msg, reply);
            }
            else
            {
                Debug.LogError("receieve message from server, no handler");
            }
        }

        public IMessagePacker messagePacker
        {
            get
            {
                return BinaryMessagePacker.Instance;
            }
        }

        public static int s_socketId = 1000;
        public int nextSocketId
        {
            get
            {
                return s_socketId++;
            }
        }

        public static int s_msgSeq = 1;
        public int nextMsgSeq
        {
            get
            {
                return s_msgSeq++;
            }
        }

        public Action<bool, string> onConnect;
        public void onConnectComplete(TcpClientData @this, SocketAsyncEventArgs e, SocketError socketError)
        {
            bool success = e.SocketError == SocketError.Success;
            if (onConnect != null)
            {
                onConnect(success, "SocketError." + e.SocketError);
            }
        }

        public Action<string> onClose;
        public void onCloseComplete(TcpClientData data)
        {
            // this.logError(@this, "TcpClientScriptS.onClose");
            if (onClose != null)
            {
                onClose("to do: message");
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////

        public void open()
        {
            this.tcpClientData.connect();
        }

        public bool isConnected()
        {
            return this.tcpClientData.isConnected();
        }

        public void send(MsgType msgType, object msg, Action<ECode, object> cb, int timeoutMs)
        {
            this.tcpClientData.send(msgType, msg, cb);
        }

        public void cleanup()
        {
            this.onConnect = null;
            this.onClose = null;
            if (!this.tcpClientData.closed)
            {
                this.tcpClientData.close("TcpClientScriptC.cleanup");
            }
        }
    }
}
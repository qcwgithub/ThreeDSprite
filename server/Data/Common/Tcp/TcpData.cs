using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Data;

namespace Data
{
    public sealed class TcpData
    {
        public ServerBaseData serverData;
        public TcpData(ServerBaseData serverData)
        {
            this.serverData = serverData;
        }

        public Socket socket;
        public SocketAsyncEventArgs listenSocketArg;
        public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
        public int msgSeq = 1;
        public int socketId = 90000;
        
        public TcpClientAcceptData acceptorData { get; set; }
        public TcpClientConnectData connectorData { get; set; }

        

        // moved to script
        // private void onComplete(SocketAsyncEventArgs e)
        // {
        //     switch (e.LastOperation)
        //     {
        //         case SocketAsyncOperation.Accept:
        //             this.onAcceptComplete(e);
        //             break;
        //         default:
        //             throw new Exception($"socket accept error: {e.LastOperation}");
        //     }
        // }

        // moved to script
        // public void acceptAsync(SocketAsyncEventArgs e)
        // {
        //     e.AcceptSocket = null;
        //     bool completed = !this.socket.AcceptAsync(e);
        //     if (completed)
        //     {
        //         // this.onAcceptComplete(e); // moved to script
        //         this.serverData.scriptProxy.onAcceptComplete(e);
        //     }
        // }

        // moved to script
        // private void onAcceptComplete(SocketAsyncEventArgs e)
        // {
        //     if (e.SocketError != SocketError.Success)
        //     {
        //         //Log.Error($"accept error {innArgs.SocketError}");
        //         this.acceptAsync(e, this.tcpData.socket);
        //         return;
        //     }

        //     bool isServer = true; // TODO
        //     var s = new TcpClientAcceptData(this.tcpData.socketId++, this.tcpData, e.AcceptSocket, isServer, this.tcpData.onConnect, this.tcpData.onDisconnect);
        //     s.start();

        //     // continue accept
        //     this.acceptAsync(e, this.tcpData.socket);
        // }

        // 此函数是多线程，因此必须放在 Data
        public void _eCompleted_multiThreaded(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(_eCompleted_mainThread, e);
        }
        private void _eCompleted_mainThread(object _e)
        {
            this.serverData.scriptProxy.onListenerSocketComplete((SocketAsyncEventArgs)_e);
        }

        // moved to script
        // public void listen(int port, Func<bool> acceptClient)
        // {
        //     var socket = this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //     var e = this.listenSocketArg = new SocketAsyncEventArgs();
        //     e.Completed += this._eCompleted_multiThreaded;

        //     socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        //     socket.Bind(new IPEndPoint(IPAddress.Any, port));
        //     socket.Listen(1000);

        //     this.acceptAsync(e);
        // }
    }
}
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

namespace Data
{
    public class TcpData
    {
        public Socket socket;

        private SocketAsyncEventArgs _listenSocketArg;
        public SocketAsyncEventArgs listenSocketArg
        {
            get { return _listenSocketArg; }
            set
            {
                if (_listenSocketArg != null)
                {
                    _listenSocketArg.Completed -= _eCompleted_multiThreaded;
                }

                _listenSocketArg = value;
                if (_listenSocketArg != null)
                {
                    _listenSocketArg.Completed += _eCompleted_multiThreaded;
                }
            }
        }
        private void _eCompleted_multiThreaded(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(_eCompleted_mainThread, e);
        }
        private void _eCompleted_mainThread(object _e)
        {
            if (this.onListenSocketComplete != null)
            {
                this.onListenSocketComplete((SocketAsyncEventArgs)_e);
            }
        }

        public Action<SocketAsyncEventArgs> onListenSocketComplete;
        public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
        public int msgSeq = 1;
        public int socketId = 90000;
        public Action<ISocket, bool> onConnect;
        public Action<ISocket, bool> onDisconnect;
    }
}
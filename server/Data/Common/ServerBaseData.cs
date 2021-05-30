using System;
using System.Collections.Generic;
using log4net;

namespace Data
{
    public abstract class ServerBaseData
    {
        public bool replyServerTime = true;

        // id: 所有服务器都不同
        public int id;
        public ScriptProxy scriptProxy;

        public TcpListenerData tcpListener;

        /////////////////////////////////////////////////////////////////////
        public TcpClientData aaaSocket;
        public TcpClientData locSocket;
        public TcpClientData dbAccountSocket;
        public TcpClientData dbPlayerSocket;
        public TcpClientData dbLogSocket;
        public Dictionary<MsgType, IHandler> handlers = new Dictionary<MsgType, IHandler>();
        public WebSocketData webSocketData = new WebSocketData();
        /////////////////////////////////////////////////////////////////////

        public Dictionary<int, Loc> knownLocs = new Dictionary<int, Loc>();

        public bool locNeedReport = true;

        public HashSet<string> locks = new HashSet<string>();

        // state
        public ServerState state = ServerState.Initing;

        public Dictionary<string, int> intMap = new Dictionary<string, int>();
        public Dictionary<string, string> stringMap = new Dictionary<string, string>();

        public int errorCount = 0;
        public int defaultDateTime = 0;

        public Random random = new Random();
        public ILog logger;

        /////////////////////////////////////////////////////////////////////
    }
}
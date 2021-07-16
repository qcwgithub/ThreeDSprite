using System;
using System.Collections.Generic;
using log4net;

namespace Data
{
    public abstract class ServerData
    {
        public bool replyServerTime = true;
        public bool grantedToStart = false;

        // id: 所有服务器都不同
        public int id;
        public ITcpListenerCallback tcpListenerCallback;
        public ITcpClientCallback tcpClientCallback;

        public TcpListenerData tcpListenerForServer;
        public TcpListenerData tcpListenerForClient;

        // 需要连接到哪些服务器
        public List<int> connectToServerIds = new List<int>();

        // 与其他服务器的连接，可能是主动连接的，也可能是被连接的
        public Dictionary<int, TcpClientData> otherServerSockets = new Dictionary<int, TcpClientData>();
        
        public int msgSeq = 1;
        public int socketId = 90000;

        public Dictionary<int, Loc> knownLocs = new Dictionary<int, Loc>();

        public bool needReportToLoc = true;

        public HashSet<string> locks = new HashSet<string>();

        // state
        public ServerState state = ServerState.Initing;

        public Dictionary<string, int> intMap = new Dictionary<string, int>();
        public int getInt(string key)
        {
            int value;
            return this.intMap.TryGetValue(key, out value) ? value : 0;
        }
        public void setInt(string key, int value)
        {
            if (value == 0)
            {
                this.intMap.Remove(key);
                return;
            }
            this.intMap[key] = value;
        }
        public Dictionary<string, string> stringMap = new Dictionary<string, string>();
        public string getString(string key)
        {
            string value;
            return this.stringMap.TryGetValue(key, out value) ? value : null;
        }
        public void setString(string key, string value)
        {
            this.stringMap[key] = value;
        }

        public int errorCount = 0;
        public int defaultDateTime = 0;

        public Random random = new Random();
        public ILog logger;

        /////////////////////////////////////////////////////////////////////

        // 临时方案
        public TimerSData timerSData;
    }
}
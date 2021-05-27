using System;
using System.Collections.Generic;

namespace Data
{
    public class WebSocketData
    {
        public Dictionary<int, Action<ECode, string>> pendingRequests = new Dictionary<int, Action<ECode, string>>();
        public int msgSeq = 1;
        public int socketId = 90000;
        public object listenerObject;
        public Dictionary<int, object> connectSockets = new Dictionary<int, object>();
    }
}
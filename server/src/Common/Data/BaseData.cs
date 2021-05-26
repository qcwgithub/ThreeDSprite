using System;
using System.Collections.Generic;

public class BaseData
{
    public bool replyServerTime = true;
    public int timezoneOffset = 0;
    public ProcessData processData = null; // 同进程共用一个

    // id: 所有服务器都不同
    public int id;

    ////// db /////
    public ISocket dbAccountSocket = null;
    public ISocket dbPlayerSocket = null;
    public ISocket dbLogSocket = null;

    public Dictionary<int, Loc> knownLocs = new Dictionary<int, Loc>();

    public ISocket locSocket = null;
    public bool locNeedReport = true;

    public HashSet<string> locks = new HashSet<string>();

    public List<Server> allServers = new List<Server>();

    // state
    public ServerState state = ServerState.Initing;

    public Dictionary<MsgType, Handler> handlers = new Dictionary<MsgType, Handler>();

    public int nextIteratorId = 1;
    public Dictionary<int, object> ianyMap = new Dictionary<int, object>();
    public Dictionary<string, object> sanyMap = new Dictionary<string, object>();

    public WebSocketData webSocketData = new WebSocketData();
    public TcpData tcpData = new TcpData();

    public int errorCount = 0;
    public int defaultDateTime = 0;

    public Random random = new Random();
}
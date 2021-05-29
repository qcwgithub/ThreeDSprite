using System;
using System.Linq;
using System.Net.Sockets;
using Data;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract class Server : IScriptProxy
    {
        public int id;
        public DataEntry dataEntry;
        public ServerBaseData baseData { get; private set; }

        public log4net.ILog logger { get; private set; }

        public BaseScript baseScript;
        public NetProtoTcp tcp;

        public MessageDispatcher dispatcher;
        public SqlLog sqlLog;
        public Utils utils;
        public JsonUtils JSON;
        public TimerScript timerScript;

        protected void AddHandler<T>() where T: Server
        {
            this.dispatcher.addHandler(new OnShutdown<T> { server = (T)this });
            this.dispatcher.addHandler(new OnConnect<T> { server = (T)this });
            this.dispatcher.addHandler(new OnDisconnect<T> { server = (T)this });
            this.dispatcher.addHandler(new KeepAliveToLoc<T> { server = (T)this });
        }

        public virtual void Create(DataEntry dataEntry, int id)
        {
            this.id = id;
            this.dataEntry = dataEntry;
            this.baseData = this.dataEntry.serverDatas[this.id];
            this.logger = this.baseData.logger;

            this.baseScript = new BaseScript { server = this };
            this.timerScript = new TimerScript { server = this };
            this.dispatcher = new MessageDispatcher { server = this };
            this.utils = new Utils();
            this.JSON = new JsonUtils();
            this.sqlLog = new SqlLog { server = this };
        }
        
        public void onListenSocketComplete(SocketAsyncEventArgs e)
        {
            this.tcp.onListenSocketComplete(e);
        }
        public void onAcceptComplete(SocketAsyncEventArgs e)
        {
            this.tcp.onAcceptComplete(e);
        }
    }
}

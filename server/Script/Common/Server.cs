using System;
using System.Linq;
using System.Net.Sockets;
using Data;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract class Server
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

        protected void AddHandler<T>() where T : Server
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
            this.baseData.scriptProxy = this.tcp;
        }

        public void onMessage(ISocket socket, bool fromServer, MsgType type, string msg, Action<ECode, string> reply)
        {
            if (!fromServer && type < MsgType.ClientStart)
            {
                this.logger.Error("receive invalid message from client! " + type.ToString());
                if (reply != null)
                {
                    reply(ECode.Exception, null);
                }
                return;
            }

            if (string.IsNullOrEmpty(msg))
            {
                this.logger.Error("message must be object!! type: " + type.ToString());
                if (reply != null)
                {
                    reply(ECode.Exception, null);
                }
                return;
            }
            // assign socket here
            // msg2.socket = s;

            // 发送方没有要求回复时，reply2 为 null
            this.dispatcher.dispatch(socket, type, msg, reply);
        }
    }
}

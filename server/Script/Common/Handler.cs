using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public abstract class Handler<T> : IHandler, IServerScript<T> where T : Server
    {
        public T server { get; set; }
        public ServerBaseData baseData { get { return this.server.baseData; } }
        public BaseScript baseScript { get { return this.server.baseScript; } }
        public log4net.ILog logger { get { return this.server.logger; } }
        public MessageDispatcher dispatcher { get { return this.server.dispatcher; } }
        public TcpClientScript tcpClientScript { get { return this.server.tcpClientScript; } }

        public abstract MsgType msgType { get; }
        public abstract Task<MyResponse> handle(TcpClientData socket, string _msg);
        public virtual void postHandle(object socket, object msg) { }
        public string msgName { get { return this.msgType.ToString(); } }
    }
}
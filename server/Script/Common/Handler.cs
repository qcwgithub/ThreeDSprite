using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        Task<MyResponse> handle(TcpClientData socket, object _msg);
        MyResponse postHandle(object socket, object msg, MyResponse r);
    }
    public abstract class Handler<T> : IHandler, IServerScript<T> where T : Server
    {
        public T server { get; set; }
        public ServerData baseData { get { return this.server.data; } }
        public log4net.ILog logger { get { return this.server.logger; } }

        public abstract MsgType msgType { get; }
        public abstract Task<MyResponse> handle(TcpClientData socket, object _msg);
        public virtual MyResponse postHandle(object socket, object msg, MyResponse r) { return r; }
        public string msgName { get { return this.msgType.ToString(); } }
    }
}
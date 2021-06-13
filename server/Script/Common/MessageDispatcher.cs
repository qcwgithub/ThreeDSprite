using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class MessageDispatcher : IServerScript<Server>
    {
        public Server server { get; set; }
        Dictionary<MsgType, IHandler> handlers = new Dictionary<MsgType, IHandler>();

        ///////// handlers ///////////
        public void addHandler(IHandler handler)
        {
            // remove first
            this.removeHandler(handler.msgType);
            this.handlers.Add(handler.msgType, handler);
        }
        public bool removeHandler(MsgType type)
        {
            return this.handlers.Remove(type);
        }
        private readonly Action<ECode, object> emptyReply = (e, r) => { };

        // about reply
        // 1 处理网络来的请求，reply 是回复请求
        // 2 自己调用 dispatch 的，reply 没什么用，为了统一，赋值为 utils.emptyReply
        // reply()的参数统一为 MyResponse
        public async void dispatch(TcpClientData socket, MsgType type, object msg, Action<ECode, object> reply)
        {
            if (reply == null)
            {
                reply = this.emptyReply;
            }

            IHandler handler;
            if (!this.handlers.TryGetValue(type, out handler))
            {
                this.server.logger.ErrorFormat("no handler for message {0}", type);
                reply(ECode.Error, null);
                return;
            }

            MyResponse r = null;
            try
            {
                r = await handler.handle(socket, msg);
            }
            catch (Exception ex)
            {
                this.server.logger.Error("disaptch exception 1! msgType: " + type, ex);
                r = new MyResponse(ECode.Exception, null);
            }

            try
            {
                r = handler.postHandle(socket, msg, r);
                reply(r.err, r.res == null ? null : r.res);
            }
            catch (Exception ex)
            {
                this.server.logger.Error("disaptch exception 2! msgType: " + type, ex);
                reply(ECode.Exception, null);
            }
        }
    }
}
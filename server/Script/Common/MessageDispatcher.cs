using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class MessageDispatcher : IServerScript<Server>
    {
        public Server server { get; set; }

        ///////// handlers ///////////
        public void addHandler<T>(Handler<T> handler) where T: Server
        {
            // remove first
            this.removeHandler(handler.msgType);
            this.server.baseData.handlers.Add(handler.msgType, handler);
        }
        public bool removeHandler(MsgType type)
        {
            return this.server.baseData.handlers.Remove(type);
        }
        private readonly Action<ECode, string> emptyReply = (e, r) => { };

        // about reply
        // 1 处理网络来的请求，reply 是回复请求
        // 2 自己调用 dispatch 的，reply 没什么用，为了统一，赋值为 utils.emptyReply
        // reply()的参数统一为 MyResponse
        public async void dispatch(TcpClientData socket, MsgType type, string msg, Action<ECode, string> reply)
        {
            if (reply == null)
            {
                reply = this.emptyReply;
            }

            IHandler handler;
            if (!this.server.baseData.handlers.TryGetValue(type, out handler))
            {
                this.server.logger.ErrorFormat("no handler for message {0}", type);
                reply(ECode.Error, null);
                return;
            }

            // try {
            // MyResponse res = new MyResponse(ECode.Error, null);
            var r = await handler.handle(socket, msg);
            handler.postHandle(socket, msg);
            reply(r.err, r.res == null ? null : this.server.JSON.stringify(r.res));
            // this.server.coroutineMgr.iterate(ie, () => {
            //     handler.postHandle(socket, msg);
            //     reply(res);
            // });
            // }
            // catch (Exception ex) {
            //     this.server.baseScript.error("dispatch exception! msgType: " + type.ToString(), ex);

            //     reply(MyResponse.exResponse);
            // }
        }
    }
}
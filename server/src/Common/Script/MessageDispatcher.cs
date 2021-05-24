using System;
using System.Collections;
using System.Threading.Tasks;

public class MessageDispatcher : IScript {
    public Server server { get; set; }

    ///////// handlers ///////////
    public void addHandler(Handler handler) {
        // remove first
        this.removeHandler(handler.msgType);
        handler.server = this.server;
        this.server.baseData.handlers.Add(handler.msgType, handler);
    }
    public Handler removeHandler(MsgType type) {
        Handler handler;
        if (!this.server.baseData.handlers.TryGetValue(type, out handler)) {
            return null;
        }
        this.server.baseData.handlers.Remove(type);
        return handler;
    }
    private readonly Action<ECode, string> emptyReply = (e,r) => { };

    // about reply
    // 1 处理网络来的请求，reply 是回复请求
    // 2 自己调用 dispatch 的，reply 没什么用，为了统一，赋值为 utils.emptyReply
    // reply()的参数统一为 MyResponse
    public async void dispatch(object socket, MsgType type, string msg, Action<ECode, string> reply) {
        if (reply == null) {
            reply = this.emptyReply;
        }

        Handler handler;
        if (!this.server.baseData.handlers.TryGetValue(type, out handler)) {
            this.server.baseScript.error("no handler for message %d, %s", type, type.ToString());
            reply(ECode.Error, null);
            return;
        }
        
        // try {
            // MyResponse res = new MyResponse(ECode.Error, null);
            var r = await handler.handle(socket, msg);
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
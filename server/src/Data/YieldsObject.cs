using System;
using System.Collections;

public interface iYieldObject {
    void setCallback(Action<MyResponse> cb);
}

public class RequestObject : iYieldObject {
    public bool @normal = true;
    private Server server;
    private object socket;
    private MsgType type;
    private object msg;
    private MyResponse res; // OUTPUT
    public RequestObject(Server server, object socket, MsgType type, object msg, MyResponse res/* OUTPUT */) {
        this.server = server;
        this.socket = socket;
        this.type = type;
        this.msg = msg;

        this.res = res;
        // init to Error
        res.err = ECode.Error;
        res.res = null;
    }

    private Action<MyResponse> cb = null;
    private bool replied = false;
    private int timer = -1;
    private void doReply(MyResponse r) {
        if (this.replied) {
            return;
        }
        this.replied = true;
        if (this.timer != -1) {
            clearTimeout(this.timer);
            this.timer = -1;
        }
        
        this.cb(r);
        this.cb = null;
    }
    
    public void setCallback(Action<MyResponse> cb) {
        this.cb = cb;
        this.server.netProto.send(this.socket, this.type, this.msg, (MyResponse r) => this.doReply(r));

        // 保底，可以考虑去掉，因为：
        // 1 客户端可以自己做 ----不行啊，有服务器的
        // 2 dispatcher中本来就保证会回复---- 也不行，他只处理没有返回值的情况
        this.timer = setTimeout(() => {
            this.doReply(new MyResponse(ECode.Timeout, null));
        }, 30000);
    }
}

public class TimeoutObject : iYieldObject {
    public bool @normal = true;
    private int ms;
    public TimeoutObject(int ms) {
        this.ms = ms;
    }
    public void setCallback(object cb) {
        setTimeout(() => {
            cb(new MyResponse(ECode.Success, this.ms));
        }, this.ms);
    }
}

// public SubCoroutine : iYieldObject {
//     private object it;
//     private mgr: CoroutineManager;
//     constructor(object it, mgr: CoroutineManager) {
//         this.it = it;
//         this.mgr = mgr;
//     }
//     setCallback(object cb): void {
//         this.mgr.add(this.it, (object r) => {
//             if (cb != null) { // 这个只是防错，不应该调用过来多次的
//                 cb(r);
//                 cb = null;
//             }
//         });
//     }
// }

public class WaitCallBack : iYieldObject {
    public bool @normal = true;
    private Action<MyResponse> cb = null;
    private Action doWhat;
    public WaitCallBack init(Action doWhat) {
        this.doWhat = doWhat;
        return this;
    }
    public void setCallback(Action<MyResponse> cb) {
        this.cb = cb;
        this.doWhat();
        this.doWhat = null;
    }
    public void finish(MyResponse r) {
        if (this.cb != null) {
            this.cb(r);
            this.cb = null;
        }
    }
}


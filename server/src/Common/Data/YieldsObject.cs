using System;
using System.Collections;
using System.Threading.Tasks;

public class RequestObject
{
    private Server server;
    private object socket;
    private MsgType type;
    private object msg;
    private TaskCompletionSource<MyResponse> completeSource;
    public Task<MyResponse> Task { get { return this.completeSource.Task; } }
    public RequestObject(Server server, object socket, MsgType type, object msg)
    {
        this.completeSource = new TaskCompletionSource<MyResponse>();
        this.server = server;
        this.socket = socket;
        this.type = type;
        this.msg = msg;
        
        this.server.baseScript.send(this.socket, this.type, this.msg, this.doReply);

        // 保底，可以考虑去掉，因为：
        // 1 客户端可以自己做 ----不行啊，有服务器的
        // 2 dispatcher中本来就保证会回复---- 也不行，他只处理没有返回值的情况
        this.timer = this.server.baseScript.setTimer(() =>
        {
            this.doReply(new MyResponse(ECode.Timeout, null));
        }, 30000);
    }
    
    private bool replied = false;
    private int timer = -1;
    private void doReply(MyResponse r)
    {
        if (this.replied)
        {
            return;
        }
        this.replied = true;
        if (this.timer != -1)
        {
            this.server.baseScript.clearTimer(this.timer);
            this.timer = -1;
        }

        this.completeSource.TrySetResult(r);
    }
}

public class WaitCallBack
{
    private TaskCompletionSource<MyResponse> completeSource;
    public Task<MyResponse> Task { get { return this.completeSource.Task; } }
    public WaitCallBack()
    {        
        this.completeSource = new TaskCompletionSource<MyResponse>();
    }
    
    public void finish(MyResponse r)
    {
        this.completeSource.TrySetResult(r);
    }
}

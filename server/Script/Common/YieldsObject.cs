using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    // public class RequestObject
    // {
    //     private Server server;
    //     private MsgType type;
    //     private object msg;
    //     private TaskCompletionSource<MyResponse> completeSource;
    //     public Task<MyResponse> Task { get { return this.completeSource.Task; } }
    //     public RequestObject(Server server, TcpClientData socket, MsgType type, object msg)
    //     {
    //         this.completeSource = new TaskCompletionSource<MyResponse>();
    //         this.server = server;
    //         this.type = type;
    //         this.msg = msg;

    //         this.server.tcpClientScript.send(socket, this.type, this.msg, this.doReply);

    //         // 保底，可以考虑去掉，因为：
    //         // 1 客户端可以自己做 ----不行啊，有服务器的
    //         // 2 dispatcher中本来就保证会回复---- 也不行，他只处理没有返回值的情况
    //         this.timer = this.server.timerScript.setTimer(() =>
    //         {
    //             this.doReply(ECode.Timeout, null);
    //         }, 30000);
    //     }

    //     private bool replied = false;
    //     private int timer = -1;
    //     private void doReply(ECode e, string r)
    //     {
    //         if (this.replied)
    //         {
    //             return;
    //         }
    //         this.replied = true;
    //         if (this.timer != -1)
    //         {
    //             this.server.timerScript.clearTimer(this.timer);
    //             this.timer = -1;
    //         }

    //         this.completeSource.TrySetResult(new MyResponse(e, r));
    //     }
    // }

    public class WaitCallBack
    {
        private TaskCompletionSource<MyResponse> completeSource;
        public Task<MyResponse> Task { get { return this.completeSource.Task; } }
        public WaitCallBack()
        {
            this.completeSource = new TaskCompletionSource<MyResponse>();
        }

        public void finish(ECode e, string r)
        {
            this.completeSource.TrySetResult(new MyResponse(e, r));
        }
    }
}
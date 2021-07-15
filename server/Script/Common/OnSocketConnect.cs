using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnSocketConnect<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.OnSocketConnect; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = (MsgOnConnect)_msg;
            this.logger.DebugFormat("{0} socket id: {1}", this.msgName, socket.getSocketId());
            // var s = socket;
            // bool isClient = !msg.isServer;
            return ECode.Success.toTask();
        }
    }
}
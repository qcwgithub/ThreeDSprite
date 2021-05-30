using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnConnect<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.OnConnect; } }

        public override async Task<MyResponse> handle(ISocket socket, string _msg)
        {
            var msg = this.baseScript.decodeMsg<MsgOnConnect>(_msg);
            this.logger.Debug("OnConnect socket id: " + socket.getId());
            // var s = socket;
            // bool isClient = !msg.isServer;
            return ECode.Success;
        }
    }
}
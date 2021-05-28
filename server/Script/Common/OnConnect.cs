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
            var s = socket;
            s.removeCustomMessageListener();

            bool isClient = !msg.isServer;

            // 消息入口
            s.setCustomMessageListener((MsgType type2, string msg2, Action<ECode, string> reply2) =>
            {
                if (isClient && type2 < MsgType.ClientStart)
                {
                    this.server.logger.Error("receive invalid message from client! " + type2.ToString());
                    if (reply2 != null)
                    {
                        reply2(ECode.Exception, null);
                    }
                    return;
                }

                if (msg2 == null)
                {
                    this.server.logger.Error("message must be object!! type: " + type2.ToString());
                    if (reply2 != null)
                    {
                        reply2(ECode.Exception, null);
                    }
                    return;
                }
                // assign socket here
                // msg2.socket = s;

                // 发送方没有要求回复时，reply2 为 null
                this.server.dispatcher.dispatch(s, type2, msg2, reply2);
            });

            return ECode.Success;
        }
    }
}
using System;
using System.Collections;
using System.Threading.Tasks;

public class OnConnect : Handler {
    public override MsgType msgType { get { return MsgType.OnConnect; } }

    public override async Task<MyResponse> handle(object socket, object _msg) {
        var msg = _msg as MsgOnConnect;
        this.logger.debug("OnConnect socket id: " + this.server.netProto.getSocketId(socket));
        object s = socket;
        this.server.netProto.removeCustomMessageListener(s);

        bool isClient = !msg.isServer;

        // 消息入口
        this.server.netProto.setCustomMessageListener(s, (MsgType type2, object msg2, Action<MyResponse> reply2) => {
            if (isClient && type2 < MsgType.ClientStart) {
                this.baseScript.error("receive invalid message from client! " + type2.ToString());
                if (reply2 != null) {
                    reply2(new MyResponse(ECode.Exception));
                }
                return;
            }

            if (msg2 == null) {
                this.baseScript.error("message must be object!! type: " + type2.ToString());
                if (reply2 != null) {
                    reply2(new MyResponse(ECode.Exception));
                }
                return;
            }
            // assign socket here
            // msg2.socket = s;

            // 发送方没有要求回复时，reply2 为 null
            this.dispatcher.dispatch(s, type2, msg2, reply2);
        });

        return ECode.Success;
    }
}
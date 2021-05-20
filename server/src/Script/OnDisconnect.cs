
using System.Collections;

public class OnDisconnect : Handler {
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse res) {
        // MsgOnConnect

        // TODO socket.id 可能是 undefined ??!
        this.logger.debug("OnDisconnect socket id: " + this.server.netProto.getSocketId(socket));

        // 如果是服务器，这里不需要 remove，因为服务器是一直尝试保持连接，需要 connect 事件，移除了就收不到了
        // 如果是客户端，这里移不移除没差
        // this.server.networkHelper.removeAllListeners(socket);
        res.err = ECode.Success;
        res.res = null;
        yield break;
    }
}
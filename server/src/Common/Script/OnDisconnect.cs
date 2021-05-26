
using System.Collections;
using System.Threading.Tasks;

public class OnDisconnect : Handler {
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg) {
        // MsgOnConnect

        // TODO socket.id 可能是 undefined ??!
        this.logger.debug("OnDisconnect socket id: " + socket.getId());

        // 如果是服务器，这里不需要 remove，因为服务器是一直尝试保持连接，需要 connect 事件，移除了就收不到了
        // 如果是客户端，这里移不移除没差
        // this.server.networkHelper.removeAllListeners(socket);
        return ECode.Success;
    }
}

// 转发消息
public class AAAOnTransfer : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAOnTransfter; }

    handle(object socket, msg: { int id, int playerCount, string ip, int port }) {
        return MyResponse.create(ECode.Success);
    }
}
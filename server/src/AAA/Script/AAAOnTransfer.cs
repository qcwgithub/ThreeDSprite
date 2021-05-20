
// 转发消息
using System.Collections;

public class AAAOnTransfer : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAOnTransfter; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        yield break;
    }
}
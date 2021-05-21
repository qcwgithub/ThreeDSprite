
// 转发消息
using System.Collections;
using System.Threading.Tasks;

public class AAAOnTransfer : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAOnTransfter; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        return ECode.Success;
    }
}
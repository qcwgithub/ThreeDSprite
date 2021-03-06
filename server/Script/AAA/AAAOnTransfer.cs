
// 转发消息
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAOnTransfer : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAAOnTransfter; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            return ECode.Success.toTask();
        }
    }
}
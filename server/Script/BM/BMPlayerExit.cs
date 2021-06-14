using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerExit : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerExit;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            return ECode.Error.toTask();
        }
    }
}
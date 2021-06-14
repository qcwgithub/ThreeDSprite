using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerEnter : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerEnter;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            return ECode.Error.toTask();
        }
    }
}
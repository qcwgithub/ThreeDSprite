
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMGetSummary : PMHandler
    {
        public override MsgType msgType { get { return MsgType.GetSummary; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.logger.Debug(this.msgName);
            return ECode.Success.toTask();
        }
    }
}
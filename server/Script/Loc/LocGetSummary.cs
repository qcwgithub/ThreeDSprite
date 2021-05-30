using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LocGetSummary : LocHandler
    {
        public override MsgType msgType { get { return MsgType.GetSummary; } }

        public override Task<MyResponse> handle(TcpClientData socket, string msg)
        {
            this.logger.Debug("LocGetSummary");
            return Task.FromResult(new MyResponse(ECode.Success, null));
        }
    }
}
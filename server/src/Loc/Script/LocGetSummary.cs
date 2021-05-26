using System.Collections.Generic;
using System.Threading.Tasks;

public class LocGetSummary : LocHandler
{
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override Task<MyResponse> handle(ISocket socket, string msg)
    {
        this.logger.debug("LocGetSummary");
        return Task.FromResult(new MyResponse(ECode.Success, null));
    }
}
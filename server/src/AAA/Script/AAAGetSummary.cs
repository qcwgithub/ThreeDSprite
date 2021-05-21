
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AAAGetSummary : AAAHandler
{
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        this.logger.debug("AAAGetSummary");
        var data = this.aaaData;

        var info = new Dictionary<string, string> {
            { "workingDir", "" },//process.cwd(),
            { "purpose", this.server.purpose.ToString() },
            { "id", this.baseData.id.ToString() },
            { "name", Utils.numberId2stringId(this.baseData.id) },
            { "playerInfos_size", data.playerInfos.Count.ToString() },
            { "playerManagerInfos_size", data.playerManagerInfos.Count.ToString() },
            { "nextPlayerId", data.nextPlayerId.ToString() },
            { "error", this.baseData.errorCount.ToString() },
            { "uncauchtException", this.baseData.processData.uncaughtExceptionCount.ToString() },
        };
        // m.accountInfos_size = data.accountInfos.size;

        return new MyResponse(ECode.Success, new List<Dictionary<string, string>> { info });
    }
}
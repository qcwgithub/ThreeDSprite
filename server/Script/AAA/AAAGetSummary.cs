
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAGetSummary : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.GetSummary; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.logger.Debug("AAAGetSummary");
            var data = this.aaaData;

            var info = new Dictionary<string, string> {
            { "workingDir", "" },//process.cwd(),
            { "purpose", this.server.dataEntry.purpose.ToString() },
            { "serverId", this.baseData.serverId.ToString() },
            { "name", Utils.numberId2stringId(this.baseData.serverId) },
            { "playerInfos_size", data.playerDict.Count.ToString() },
            { "playerManagerInfos_size", data.playerManagerDict.Count.ToString() },
            { "nextPlayerId", data.nextPlayerId.ToString() },
            { "error", this.baseData.errorCount.ToString() },
            { "uncauchtException", this.server.dataEntry.processData.uncaughtExceptionCount.ToString() },
        };
            // m.accountInfos_size = data.accountInfos.size;

            return new MyResponse(ECode.Success, new List<Dictionary<string, string>> { info }).toTask();
        }
    }
}
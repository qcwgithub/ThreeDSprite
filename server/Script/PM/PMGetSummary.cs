
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMGetSummary : PMHandler
    {
        public override MsgType msgType { get { return MsgType.GetSummary; } }

        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            this.logger.Debug("PMGetSummary");
            return ECode.Success;

            // var info = {
            //     workingDir: process.cwd(),
            //     purpose: Purpose[this.server.purpose],
            //     id: this.baseData.id,
            //     name: Utils.numberId2stringId(this.baseData.id),
            //     onlineCount: 0,
            //     offlineCount: 0,
            //     error: this.baseData.errorCount,
            //     uncauchtException: this.baseData.processData.uncaughtExceptionCount
            // };
            // this.pmData.playerInfos.forEach((player, _id, _) => {
            //     if (this.server.netProto.isConnected(player.socket)) {
            //         info.onlineCount++;
            //     }
            //     else {
            //         info.offlineCount++;
            //     }
            // });
            // return new MyResponse(ECode.Success, [info]);
        }
    }
}
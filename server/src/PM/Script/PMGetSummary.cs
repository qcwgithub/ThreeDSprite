
using System.Collections;

public class PMGetSummary : PMHandler {
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        this.logger.debug("PMGetSummary");
        yield break;

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
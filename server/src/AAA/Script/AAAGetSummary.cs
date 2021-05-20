
using System.Collections;
using System.Collections.Generic;

public class AAAGetSummary : AAAHandler {
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r) {
        this.logger.debug("AAAGetSummary");
        var data = this.aaaData;

        object info = {
            workingDir: process.cwd(),
            purpose: Purpose[this.server.purpose],
            id: this.baseData.id,
            name: Utils.numberId2stringId(this.baseData.id),
            playerInfos_size: data.playerInfos.size,
            playerManagerInfos_size: data.playerManagerInfos.size,
            nextPlayerId: data.nextPlayerId.toString(),
            error: this.baseData.errorCount,
            uncauchtException: this.baseData.processData.uncaughtExceptionCount,
        };
        // m.accountInfos_size = data.accountInfos.size;

        return new MyResponse(ECode.Success, [info]);
    }
}
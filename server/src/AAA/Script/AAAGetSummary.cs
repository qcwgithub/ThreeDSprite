
public class AAAGetSummary : AAAHandler {
    public override MsgType msgType { get { return MsgType.GetSummary; }

    *handle(object socket, object msg/* no use */): any {
        this.logger.debug("AAAGetSummary");
        var data = this.aaaData;

        var object info = {
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
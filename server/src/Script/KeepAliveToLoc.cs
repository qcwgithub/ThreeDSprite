public class KeepAliveToLoc : Handler {

    *handle(object socket, msg: { boolean isListen, boolean isServer }) {
        if (!this.server.netProto.isConnected(this.baseData.locSocket)) {
            this.baseData.locNeedReport = true;
            r.err = ECode.Success;
        yield break;
        }

        if (this.baseData.locNeedReport) {
            this.baseData.locNeedReport = false;

            yield return this.baseScript.sendYield(this.baseData.locSocket, MsgType.LocReportLoc, { id: this.baseData.id, loc: this.baseScript.myLoc() }, r);
            if (r.err != ECode.Success) {
                console.error("!canStart: " + ECode[r.err]);
                process.exit(1);
            }
        }

        r.err = ECode.Success;
        yield break;
    }
}
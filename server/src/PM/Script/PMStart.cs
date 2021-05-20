
using System.Collections;

public class PMStart : PMHandler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public override IEnumerator handle(object socket, object msg/* no use */, MyResponse r) {
        var data = this.pmData;
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        yield return this.baseScript.connectYield(ServerConst.LOC_ID, true, r);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        // request location(s)
        yield return this.baseScript.requestLocationYield(new int[] { ServerConst.AAA_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID }, r);

        // connect to dbPlayer
        yield return this.baseScript.connectYield(ServerConst.DB_PLAYER_ID, true, r);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        yield return this.baseScript.connectYield(ServerConst.DB_LOG_ID, true, r);
        this.baseData.dbLogSocket = r.res;

        // connect to AAA
        yield return this.baseScript.connectYield(ServerConst.AAA_ID, true, r);
        data.aaaSocket = r.res;

        data.alive.timer = this.baseScript.setTimerLoop(1000, MsgType.PMKeepAliveToAAA, new object());

        //this.dispatcher.dispatch(MsgType.PMPayiOSTest, {}, null);

        this.baseScript.listen(() => this.server.pmScript.acceptClient());
        this.baseScript.setState(ServerState.Started);
        r.err = ECode.Success;
        yield break;
    }
}
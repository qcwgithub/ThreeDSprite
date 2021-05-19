
public class PMStart : PMHandler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public override MyResponse handle(object socket, object msg/* no use */) {
        var data = this.pmData;
        this.baseScript.setState(ServerState.Starting);
        MyResponse r = null;

        // connect to loc
        r = yield this.baseScript.connectYield(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, {});

        // request location(s)
        yield this.baseScript.requestLocationYield([ServerConst.AAA_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID]);

        // connect to dbPlayer
        r = yield this.baseScript.connectYield(ServerConst.DB_PLAYER_ID);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        r = yield this.baseScript.connectYield(ServerConst.DB_LOG_ID);
        this.baseData.dbLogSocket = r.res;

        // connect to AAA
        r = yield this.baseScript.connectYield(ServerConst.AAA_ID);
        data.aaaSocket = r.res;

        data.alive.timer = this.baseScript.setTimerLoop(1000, MsgType.PMKeepAliveToAAA, {});

        //this.dispatcher.dispatch(MsgType.PMPayiOSTest, {}, null);

        this.baseScript.listen(() => this.server.pmScript.acceptClient());
        this.baseScript.setState(ServerState.Started);
        return MyResponse.create(ECode.Success);
    }
}
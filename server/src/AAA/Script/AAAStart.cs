
public class AAAStart : AAAHandler {
    public override MsgType msgType { get { return MsgType.Start; }

    *handle(object socket, object msg/* no use */) {
        this.baseScript.setState(ServerState.Starting);
        MyResponse r = null;

        // connect to loc
        r = yield this.baseScript.connectYield(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, {});

        // request location(s)
        yield this.baseScript.requestLocationYield([ServerConst.DB_ACCOUNT_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID]);

        // connect to dbAccount
        r = yield this.baseScript.connectYield(ServerConst.DB_ACCOUNT_ID);
        this.baseData.dbAccountSocket = r.res;

        // connect to dbPlayer
        r = yield this.baseScript.connectYield(ServerConst.DB_PLAYER_ID);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        r = yield this.baseScript.connectYield(ServerConst.DB_LOG_ID);
        this.baseData.dbLogSocket = r.res;

        // load next player id
        yield this.baseScript.sendToSelfYield(MsgType.AAALoadPlayerId, {});

        // 
        this.baseScript.sendToSelf(MsgType.AAAPayLtListenNotify, {});
        this.baseScript.sendToSelf(MsgType.AAAPayIvyListenNotify, {});

        // listen
        this.baseScript.listen(() => this.server.aaaScript.acceptClient());

        this.baseScript.setState(ServerState.Started);
        return MyResponse.create(ECode.Success);
    }
}
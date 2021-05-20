
using System.Collections;

public class AAAStart : AAAHandler
{
    public override MsgType msgType { get { return MsgType.Start; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        yield return this.baseScript.connectYield(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, {});

        // request location(s)
        yield return this.baseScript.requestLocationYield(new int[] { ServerConst.DB_ACCOUNT_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID }, r);

        // connect to dbAccount
        yield return this.baseScript.connectYield(ServerConst.DB_ACCOUNT_ID, true, r);
        this.baseData.dbAccountSocket = r.res;

        // connect to dbPlayer
        yield return this.baseScript.connectYield(ServerConst.DB_PLAYER_ID, true, r);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        yield return this.baseScript.connectYield(ServerConst.DB_LOG_ID, true, r);
        this.baseData.dbLogSocket = r.res;

        // load next player id
        yield return this.baseScript.sendToSelfYield(MsgType.AAALoadPlayerId, {});

        // 
        this.baseScript.sendToSelf(MsgType.AAAPayLtListenNotify, {});
        this.baseScript.sendToSelf(MsgType.AAAPayIvyListenNotify, {});

        // listen
        this.baseScript.listen(() => this.server.aaaScript.acceptClient());

        this.baseScript.setState(ServerState.Started);
        r.err = ECode.Success;
    }
}

using System.Collections;
using System.Threading.Tasks;

public class AAAStart : AAAHandler
{
    public override MsgType msgType { get { return MsgType.Start; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        MyResponse r = null;
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        r = await this.baseScript.connectAsync(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        // request location(s)
        r = await this.baseScript.requestLocationYield(new int[] { ServerConst.DB_ACCOUNT_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID });

        // connect to dbAccount
        r = await this.baseScript.connectAsync(ServerConst.DB_ACCOUNT_ID);
        this.baseData.dbAccountSocket = r.res;

        // connect to dbPlayer
        r = await this.baseScript.connectAsync(ServerConst.DB_PLAYER_ID);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        r = await this.baseScript.connectAsync(ServerConst.DB_LOG_ID);
        this.baseData.dbLogSocket = r.res;

        // load next player id
        r = await this.baseScript.sendToSelfYield(MsgType.AAALoadPlayerId, new object());

        // 
        this.baseScript.sendToSelf(MsgType.AAAPayLtListenNotify, new object());
        this.baseScript.sendToSelf(MsgType.AAAPayIvyListenNotify, new object());

        // listen
        this.baseScript.listen(() => this.server.aaaScript.acceptClient());

        this.baseScript.setState(ServerState.Started);
        return ECode.Success;
    }
}
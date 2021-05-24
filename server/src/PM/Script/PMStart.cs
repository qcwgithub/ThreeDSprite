
using System.Collections;
using System.Threading.Tasks;

public class PMStart : PMHandler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public override async Task<MyResponse> handle(object socket, string msg/* no use */) {
        var data = this.pmData;
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        var r = await this.baseScript.connectAsync(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        // request location(s)
        r = await this.baseScript.requestLocationYield(new int[] { ServerConst.AAA_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID });

        // connect to dbPlayer
        r = await this.baseScript.connectAsync(ServerConst.DB_PLAYER_ID);
        this.baseData.dbPlayerSocket = r.res;

        // connect to dbLog
        r = await this.baseScript.connectAsync(ServerConst.DB_LOG_ID);
        this.baseData.dbLogSocket = r.res;

        // connect to AAA
        r = await this.baseScript.connectAsync(ServerConst.AAA_ID);
        data.aaaSocket = r.res;

        data.alive.timer = this.baseScript.setTimerLoop(1000, MsgType.PMKeepAliveToAAA, new object());

        //this.dispatcher.dispatch(MsgType.PMPayiOSTest, {}, null);

        this.baseScript.listen(() => this.server.pmScript.acceptClient());
        this.baseScript.setState(ServerState.Started);
        return ECode.Success;
    }
}
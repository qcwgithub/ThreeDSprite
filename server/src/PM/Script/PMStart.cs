
using System.Collections;
using System.Threading.Tasks;

public class PMStart : PMHandler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public override async Task<MyResponse> handle(ISocket socket, string msg/* no use */) {
        var data = this.pmData;
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        this.baseData.locSocket = await this.baseScript.connectAsync(ServerConst.LOC_ID);
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        // request location(s)
        await this.baseScript.requestLocationAsync(new int[] { ServerConst.AAA_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID });

        // connect to dbPlayer
        this.baseData.dbPlayerSocket = await this.baseScript.connectAsync(ServerConst.DB_PLAYER_ID);

        // connect to dbLog
        this.baseData.dbLogSocket = await this.baseScript.connectAsync(ServerConst.DB_LOG_ID);

        // connect to AAA
        data.aaaSocket = await this.baseScript.connectAsync(ServerConst.AAA_ID);

        data.alive.timer = this.baseScript.setTimerLoop(1000, MsgType.PMKeepAliveToAAA, new object());

        //this.dispatcher.dispatch(MsgType.PMPayiOSTest, {}, null);

        this.baseScript.listen(() => this.server.pmScript.acceptClient());
        this.baseScript.setState(ServerState.Started);
        return ECode.Success;
    }
}
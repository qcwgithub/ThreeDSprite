
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAStart : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.Start; } }

        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            MyResponse r = null;
            this.baseScript.setState(ServerState.Starting);

            // connect to loc
            this.baseData.locSocket = await this.baseScript.connectAsync(ServerConst.LOC_ID);
            this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

            // request location(s)
            r = await this.baseScript.requestLocationAsync(new int[] { ServerConst.DB_ACCOUNT_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID });

            // connect to dbAccount
            this.baseData.dbAccountSocket = await this.baseScript.connectAsync(ServerConst.DB_ACCOUNT_ID);

            // connect to dbPlayer
            this.baseData.dbPlayerSocket = await this.baseScript.connectAsync(ServerConst.DB_PLAYER_ID);

            // connect to dbLog
            this.baseData.dbLogSocket = await this.baseScript.connectAsync(ServerConst.DB_LOG_ID);

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
}
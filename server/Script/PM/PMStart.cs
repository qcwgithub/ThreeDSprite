
// using System.Collections;
// using System.Threading.Tasks;
// using Data;

// namespace Script
// {
//     public class PMStart : PMHandler
//     {
//         public override MsgType msgType { get { return MsgType.Start; } }

//         public override async Task<MyResponse> handle(TcpClientData socket, string msg/* no use */)
//         {
//             var data = this.data;
//             this.baseScript.setState(ServerState.Starting);

//             // connect to loc
//             this.baseData.locSocket = await this.baseScript.connectAsync(ServerConst.LOC_ID);
//             this.server.timerScript.setTimer(1000, MsgType.KeepAliveToLoc, null, true);

//             // request location(s)
//             await this.baseScript.requestLocAsync(new int[] { ServerConst.AAA_ID, ServerConst.DB_PLAYER_ID, ServerConst.DB_LOG_ID });

//             // connect to dbPlayer
//             this.baseData.dbPlayerSocket = await this.baseScript.connectAsync(ServerConst.DB_PLAYER_ID);

//             // connect to dbLog
//             this.baseData.dbLogSocket = await this.baseScript.connectAsync(ServerConst.DB_LOG_ID);

//             // connect to AAA
//             data.aaaSocket = await this.baseScript.connectAsync(ServerConst.AAA_ID);

//             data.alive.timer = this.server.timerScript.setTimer(1000, MsgType.PMKeepAliveToAAA, null, true);

//             //this.dispatcher.dispatch(MsgType.PMPayiOSTest, {}, null);

//             this.server.tcpListenerScript.listen(() => this.server.pmScript.acceptClient());
//             this.baseScript.setState(ServerState.Started);
//             return ECode.Success;
//         }
//     }
// }
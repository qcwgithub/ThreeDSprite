
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMSendDestroyPlayer : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMSendDestroyPlayer; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgSendDestroyPlayer>(_msg);
            var data = this.data;
            PMPlayer player = data.GetPlayerInfo(msg.playerId);
            if (player == null)
            {
                this.server.logger.ErrorFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
                return ECode.PlayerNotExist.toTask();
            }

            this.pmScript.clearDestroyTimer(player, false);
            
            this.logger.Info("send destroy playerId: " + player.id);
            MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = player.id, place = "pmDestroyTimer" };
            this.server.tcpClientScript.sendToServer(ServerConst.AAA_ID, MsgType.AAADestroyPlayer, msgDestroy, null);
            return ECode.Success.toTask();
        }
    }
}
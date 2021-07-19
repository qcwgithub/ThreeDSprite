
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMDestroyPlayer : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMDestroyPlayer; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgDestroyPlayer>(_msg);

            var data = this.data;
            var script = this.pmScript;
            var logger = this.logger;

            this.logger.InfoFormat("{0} place: {1}, playerId: {2}, preCount: {3}", this.msgName, msg.place, msg.playerId, data.playerInfos.Count);

            PMPlayer player = data.GetPlayerInfo(msg.playerId);
            if (player == null)
            {
                logger.InfoFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
                return ECode.PlayerNotExist.toTask();
            }

            if (player.socket != null)
            {
                player.socket.close("PMDestroyPlayer"); // PMOnDisconnect
            }

            script.clearDestroyTimer(player, false);

            // 保存一次
            script.clearSaveTimer(player);
            var msgSave = new MsgPlayerSCSave { playerId = player.id, place = this.msgName };
            this.server.proxyDispatch(null, MsgType.PMPlayerSave, msgSave, null);

            data.playerInfos.Remove(msg.playerId);
            return ECode.Success.toTask();
        }
    }
}
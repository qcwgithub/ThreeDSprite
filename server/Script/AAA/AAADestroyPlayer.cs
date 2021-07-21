
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAADestroyPlayer : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAADestroyPlayer; } }

        public override Task<MyResponse> handle(TcpClientData socket/* null */, object _msg)
        {
            var msg = this.server.CastObject<MsgDestroyPlayer>(_msg);
            var aaaData = this.aaaData;
            var aaaScript = this.aaaScript;
            this.logger.InfoFormat("{0} place: {1}, playerId: {2}, preCount: {3}", this.msgName, msg.place, msg.playerId, aaaData.playerDict.Count);

            var playerlock = "player_" + msg.playerId;
            // if (this.baseScript.isLocked(playerlock))
            // {
            //     this.logger.InfoFormat("{0} player is busy, playerId: {1}", this.msgName, msg.playerId);
            //     return ECode.PlayerLock.toTask();
            // }

            AAAPlayer player = aaaData.GetPlayer(msg.playerId);
            if (player == null)
            {
                this.server.logger.ErrorFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
                return new MyResponse(ECode.PlayerNotExist).toTask();
            }

            aaaData.playerDict.Remove(msg.playerId);

            if (player.pmId > 0)
            {
                var pm = aaaData.GetPlayerManager(player.pmId);
                if (pm != null)
                {
                    var msgPm = new MsgDestroyPlayer { playerId = player.playerId, place = msg.place };
                    this.server.tcpClientScript.sendToServer(pm.pmId, MsgType.PMDestroyPlayer, msgPm, null);
                }
                else
                {
                    this.server.logger.ErrorFormat("{0} player pm is null, playerId: {1}, pmId: {2}", this.msgName, msg.playerId, player.pmId);
                }
            }

            return ECode.Success.toTask();
        }
    }
}
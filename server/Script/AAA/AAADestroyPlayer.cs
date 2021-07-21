
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

            AAAPlayer playerInfo = aaaData.GetPlayer(msg.playerId);
            if (playerInfo == null)
            {
                this.server.logger.ErrorFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
                return new MyResponse(ECode.PlayerNotExist).toTask();
            }

            aaaData.playerDict.Remove(msg.playerId);

            if (playerInfo.pmId > 0)
            {
                var pmInfo = aaaData.GetPlayerManagerInfo(playerInfo.pmId);
                if (pmInfo != null)
                {
                    var msgPm = new MsgDestroyPlayer { playerId = playerInfo.playerId, place = msg.place };
                    this.server.tcpClientScript.sendToServer(pmInfo.pmId, MsgType.PMDestroyPlayer, msgPm, null);
                }
                else
                {
                    this.server.logger.ErrorFormat("{0} player pm is null, playerId: {1}, pmId: {2}", this.msgName, msg.playerId, playerInfo.pmId);
                }
            }

            return ECode.Success.toTask();
        }
    }
}
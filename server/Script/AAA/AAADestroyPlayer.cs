
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAADestroyPlayer : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAADestroyPlayer; } }

        public override Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            var msg = this.baseScript.decodeMsg<MsgDestroyPlayer>(_msg);
            var aaaData = this.aaaData;
            var aaaScript = this.aaaScript;
            this.logger.InfoFormat("{0} place: {1}, playerId: {2}, preCount: {3}", this.msgName, msg.place, msg.playerId, aaaData.playerInfos.Count);

            var playerlock = "player_" + msg.playerId;
            if (this.baseScript.isLocked(playerlock))
            {
                this.logger.InfoFormat("{0} player is busy, playerId: {1}", this.msgName, msg.playerId);
                return Task.FromResult(new MyResponse(ECode.PlayerLock));
            }

            AAAPlayerInfo playerInfo = aaaData.GetPlayerInfo(msg.playerId);
            if (playerInfo == null)
            {
                this.server.logger.ErrorFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
                return Task.FromResult(new MyResponse(ECode.PlayerNotExist));
            }

            aaaData.playerInfos.Remove(msg.playerId);

            if (playerInfo.pmId > 0)
            {
                var pmInfo = aaaData.GetPlayerManagerInfo(playerInfo.pmId);
                if (pmInfo != null)
                {
                    var msgPm = new MsgDestroyPlayer { playerId = playerInfo.id, place = msg.place };
                    this.tcpClientScript.send(pmInfo.socket, MsgType.PMDestroyPlayer, msgPm, null);
                }
                else
                {
                    this.server.logger.ErrorFormat("{0} player pm is null, playerId: {1}, pmId: {2}", this.msgName, msg.playerId, playerInfo.pmId);
                }
            }

            return Task.FromResult(new MyResponse(ECode.Success));
        }
    }
}
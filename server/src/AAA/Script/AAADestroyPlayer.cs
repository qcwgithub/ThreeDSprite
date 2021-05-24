
using System.Collections;
using System.Threading.Tasks;

public class AAADestroyPlayer : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAADestroyPlayer; } }

    public override async Task<MyResponse> handle(object socket, string _msg) {
        var msg = this.baseScript.castMsg<MsgDestroyPlayer>(_msg);
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;
        this.logger.info("%s place: %s, playerId: %d, preCount: %d", this.msgName, msg.place, msg.playerId, aaaData.playerInfos.Count);

        var playerlock = "player_" + msg.playerId;
        if (this.baseScript.isLocked(playerlock)) {
            this.logger.info("%s player is busy, playerId: %d", this.msgName, msg.playerId);
            return ECode.PlayerLock;
        }

        AAAPlayerInfo playerInfo = aaaData.GetPlayerInfo(msg.playerId);
        if (playerInfo == null) {
            this.baseScript.error("%s player not exit, playerId: %d", this.msgName, msg.playerId);
            return ECode.PlayerNotExist;
        }

        aaaData.playerInfos.Remove(msg.playerId);

        if (playerInfo.pmId > 0) {
            var pmInfo = aaaData.GetPlayerManagerInfo(playerInfo.pmId);
            if (pmInfo != null) {
                var msgPm = new MsgDestroyPlayer { playerId = playerInfo.id, place = msg.place };
                this.baseScript.send(pmInfo.socket, MsgType.PMDestroyPlayer, msgPm, null);
            }
            else {
                this.baseScript.error("%s player pm is null, playerId: %d, pmId: %d", this.msgName, msg.playerId, playerInfo.pmId);
            }
        }

        return ECode.Success;
    }
}
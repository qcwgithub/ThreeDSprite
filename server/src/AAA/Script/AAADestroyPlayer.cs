
using System.Collections;

public class AAADestroyPlayer : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAADestroyPlayer; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r) {
        var msg = _msg as MsgDestroyPlayer;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;
        this.logger.info("%s place: %s, playerId: %d, preCount: %d", this.msgName, msg.place, msg.playerId, aaaData.playerInfos.size);

        var playerlock = "player_" + msg.playerId;
        if (this.baseScript.isLocked(playerlock)) {
            this.logger.info("%s player is busy, playerId: %d", this.msgName, msg.playerId);
            r.err = ECode.PlayerLock;
            yield break;
        }

        AAAPlayerInfo playerInfo = aaaData.GetPlayerInfo(msg.playerId);
        if (playerInfo == null) {
            this.baseScript.error("%s player not exit, playerId: %d", this.msgName, msg.playerId);
            r.err = ECode.PlayerNotExist;
            yield break;
        }

        aaaData.playerInfos.Remove(msg.playerId);

        if (playerInfo.pmId > 0) {
            var pmInfo = aaaData.playerManagerInfos.get(playerInfo.pmId);
            if (pmInfo != null) {
                var msgPm = new MsgDestroyPlayer { playerId = playerInfo.id, place = msg.place };
                this.server.netProto.send(pmInfo.socket, MsgType.PMDestroyPlayer, msgPm, null);
            }
            else {
                this.baseScript.error("%s player pm is null, playerId: %d, pmId: %d", this.msgName, msg.playerId, playerInfo.pmId);
            }
        }

        r.err = ECode.Success;
        r.res = null;
    }
}
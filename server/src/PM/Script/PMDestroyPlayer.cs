
public class PMDestroyPlayer : PMHandler {
    public override MsgType msgType { get { return MsgType.PMDestroyPlayer; } }

    public override MyResponse handle(object socket, object _msg) {
        var msg = _msg as MsgDestroyPlayer;

        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.info("%s place: %s, playerId: %d, preCount: %d", this.msgName, msg.place, msg.playerId, data.playerInfos.size);

        PMPlayerInfo player = data.GetPlayerInfo(msg.playerId);
        if (player == null) {
            logger.info("%s player not exit, playerId: %d", this.msgName, msg.playerId);
            return MyResponse.create(ECode.PlayerNotExist);
        }

        if (player.socket != null) {
            this.server.netProto.closeSocket(player.socket); // 期望是走到 PMOnDisconnect
        }

        script.clearDestroyTimer(player, false);

        // 保存一次
        script.clearSaveTimer(player);
        MsgPlayerSCSave msgSave = { playerId: player.id, place: this.msgName };
        this.baseScript.sendToSelf(MsgType.PMPlayerSave, msgSave); // 同步

        data.playerInfos.delete(msg.playerId);
        return MyResponse.create(ECode.Success);
    }
}
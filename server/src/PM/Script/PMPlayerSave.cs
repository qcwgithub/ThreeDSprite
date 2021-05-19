using System.Collections.Generic;

public class PMPlayerSave : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPlayerSave; } }
    public override MyResponse handle(object socket, MsgPlayerSCSave msg) {
        var player = this.pmData.GetPlayerInfo(msg.playerId);
        if (player == null) {
            this.baseScript.error("%s place: %s, playerId: %d, player == null!!", this.msgName, msg.place, msg.playerId);
            return MyResponse.create(ECode.PlayerNotExist);
        }

        var obj = this.server.pmSqlUtils.beginSave(player);
        List<string> buffer = new List<string>();
        var last = player.lastProfile;
        var curr = this.server.pmPlayerToSqlTablePlayer.convert(player);

        this.server.pmSqlUtils.endSave(obj);
        player.lastProfile = curr; // 先假设一定成功吧

        this.logger.info("%s place: %s, playerId: %d, fields: [%s]", this.msgName, msg.place, player.id, buffer ? buffer.join(",") : "");

        //// reply
        return MyResponse.create(ECode.Success);
    }
}
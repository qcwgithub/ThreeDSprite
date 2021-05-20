using System.Collections;
using System.Collections.Generic;

public class PMPlayerSave : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPlayerSave; } }
    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgPlayerSCSave;
        var player = this.pmData.GetPlayerInfo(msg.playerId);
        if (player == null) {
            this.baseScript.error("%s place: %s, playerId: %d, player == null!!", this.msgName, msg.place, msg.playerId);
            r.err = ECode.PlayerNotExist;
            yield break;
        }

        var obj = this.server.pmSqlUtils.beginSave(player);
        List<string> buffer = null;
        var last = player.lastProfile;
        var curr = this.server.pmPlayerToSqlTablePlayer.convert(player);

        this.server.pmSqlUtils.endSave(obj);
        player.lastProfile = curr; // 先假设一定成功吧

        string fieldsStr = "";
        if (buffer != null)
        {
            fieldsStr = string.Join('', buffer.ToArray());
        }
        this.logger.info("%s place: %s, playerId: %d, fields: [%s]", this.msgName, msg.place, player.id, fieldsStr);

        //// reply
        r.err = ECode.Success;
        yield break;
    }
}
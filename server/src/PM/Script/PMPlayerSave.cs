using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PMPlayerSave : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPlayerSave; } }
    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        var msg = _msg as MsgPlayerSCSave;
        var player = this.pmData.GetPlayerInfo(msg.playerId);
        if (player == null) {
            this.baseScript.error("%s place: %s, playerId: %d, player == null!!", this.msgName, msg.place, msg.playerId);
            return ECode.PlayerNotExist;
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
            fieldsStr = string.Join(null, buffer.ToArray());
        }
        this.logger.info("%s place: %s, playerId: %d, fields: [%s]", this.msgName, msg.place, player.id, fieldsStr);

        //// reply
        return ECode.Success;
    }
}
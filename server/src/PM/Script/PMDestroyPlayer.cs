
using System.Collections;

public class PMDestroyPlayer : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMDestroyPlayer; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgDestroyPlayer;

        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.info("%s place: %s, playerId: %d, preCount: %d", this.msgName, msg.place, msg.playerId, data.playerInfos.Count);

        PMPlayerInfo player = data.GetPlayerInfo(msg.playerId);
        if (player == null)
        {
            logger.info("%s player not exit, playerId: %d", this.msgName, msg.playerId);
            r.err = ECode.PlayerNotExist;
            yield break;
        }

        if (player.socket != null)
        {
            this.server.netProto.closeSocket(player.socket); // PMOnDisconnect
        }
        
        script.clearDestroyTimer(player, false);

        // 保存一次
        script.clearSaveTimer(player);
        var msgSave = new MsgPlayerSCSave { playerId = player.id, place = this.msgName };
        this.baseScript.sendToSelf(MsgType.PMPlayerSave, msgSave); // 同步

        data.playerInfos.Remove(msg.playerId);
        r.err = ECode.Success;
        yield break;
    }
}
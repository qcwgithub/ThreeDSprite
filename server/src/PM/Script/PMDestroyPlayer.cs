
using System.Collections;
using System.Threading.Tasks;

public class PMDestroyPlayer : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMDestroyPlayer; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        var msg = this.baseScript.castMsg<MsgDestroyPlayer>(_msg);

        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.info("%s place: %s, playerId: %d, preCount: %d", this.msgName, msg.place, msg.playerId, data.playerInfos.Count);

        PMPlayerInfo player = data.GetPlayerInfo(msg.playerId);
        if (player == null)
        {
            logger.info("%s player not exit, playerId: %d", this.msgName, msg.playerId);
            return ECode.PlayerNotExist;
        }

        if (player.socket != null)
        {
            this.server.network.closeSocket(player.socket); // PMOnDisconnect
        }
        
        script.clearDestroyTimer(player, false);

        // 保存一次
        script.clearSaveTimer(player);
        var msgSave = new MsgPlayerSCSave { playerId = player.id, place = this.msgName };
        this.baseScript.sendToSelf(MsgType.PMPlayerSave, msgSave); // 同步

        data.playerInfos.Remove(msg.playerId);
        return ECode.Success;
    }
}
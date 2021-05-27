
using System.Collections;
using System.Threading.Tasks;
using Data;

public class PMDestroyPlayer : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMDestroyPlayer; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgDestroyPlayer>(_msg);

        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.InfoFormat("{0} place: {1}, playerId: {2}, preCount: {3}", this.msgName, msg.place, msg.playerId, data.playerInfos.Count);

        PMPlayerInfo player = data.GetPlayerInfo(msg.playerId);
        if (player == null)
        {
            logger.InfoFormat("{0} player not exit, playerId: {1}", this.msgName, msg.playerId);
            return ECode.PlayerNotExist;
        }

        if (player.socket != null)
        {
            player.socket.close(); // PMOnDisconnect
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
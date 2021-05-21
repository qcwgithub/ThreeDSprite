
using System.Collections;
using System.Threading.Tasks;

public class PMChangeChannel : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMChangeChannel; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        var msg = _msg as MsgChangeChannel;
        var player = this.server.netProto.getPlayer(socket);
        if (player == null)
        {
            return ECode.PlayerNotExist;
        }

        this.logger.info("%s playerId: %d, (%s,%s) -> (%s,%s)", this.msgName, player.id, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);

        var resPM = new ResChangeChannel();
        var err = this.server.gameScript.changeChannelCheck(player, msg, resPM);
        if (err != ECode.Success)
        {
            this.pmScript.playerOperError(this, player.id, err);
            return err;
        }

        msg.playerId = player.id;

        var r = await this.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAAChangeChannel, msg);
        if (r.err != ECode.Success)
        {
            return r;
        }

        var resAAA = r.res as ResChangeChannel;
        resPM.channel2Exist = resAAA.channel2Exist;
        resPM.userName = resAAA.userName;
        if (!resPM.channel2Exist)
        { // 如果只是切换账号，不算登录奖励
            resPM.loginReward = 1;
        }

        this.server.gameScript.changeChannelExecute(player, msg, resPM);
        return new MyResponse(ECode.Success, resPM);
    }
}
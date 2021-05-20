
using System.Collections;

public class PMChangeChannel : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMChangeChannel; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgChangeChannel;
        var player = this.server.netProto.getPlayer(socket);
        if (player == null)
        {
            r.err = ECode.PlayerNotExist;
            yield break;
        }

        this.logger.info("%s playerId: %d, (%s,%s) -> (%s,%s)", this.msgName, player.id, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);

        var resPM = new ResChangeChannel();
        r.err = this.server.gameScript.changeChannelCheck(player, msg, resPM);
        if (r.err != ECode.Success)
        {
            this.pmScript.playerOperError(this, player.id, r.err);
            yield break;
        }

        msg.playerId = player.id;

        yield return this.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAAChangeChannel, msg, r);
        if (r.err != ECode.Success)
        {
            yield break;
        }

        var resAAA = r.res as ResChangeChannel;
        resPM.channel2Exist = resAAA.channel2Exist;
        resPM.userName = resAAA.userName;
        if (!resPM.channel2Exist)
        { // 如果只是切换账号，不算登录奖励
            resPM.loginReward = 1;
        }

        this.server.gameScript.changeChannelExecute(player, msg, resPM);
        r.err = ECode.Success;
        r.res = resPM;
    }
}
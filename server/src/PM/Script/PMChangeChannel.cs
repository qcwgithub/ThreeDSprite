
public class PMChangeChannel : PMHandler {
    public override MsgType msgType { get { return MsgType.PMChangeChannel; } }
    *handle(object socket, MsgChangeChannel msg) {
        var player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }

        this.logger.info("%s playerId: %d, (%s,%s) -> (%s,%s)", this.msgName, player.id, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);

        var resPM = new ResChangeChannel();
        var e = this.server.gameScript.changeChannelCheck(player, msg, resPM);
        if (e != ECode.Success) {
            this.pmScript.playerOperError(this, player.id, e);
            return e;
        }

        msg.playerId = player.id;

        MyResponse r = yield this.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAAChangeChannel, msg);
        if (r.err == ECode.Success) {
            var resAAA = r.res as ResChangeChannel;
            resPM.channel2Exist = resAAA.channel2Exist;
            resPM.userName = resAAA.userName;
            if (!resPM.channel2Exist) { // 如果只是切换账号，不算登录奖励
                resPM.loginReward = 1;
            }

            this.server.gameScript.changeChannelExecute(player, msg, resPM);
            if (resPM.loginReward > 0) {
                player.profileChanged(PMProfileType.loginReward);
            }

            return new MyResponse(ECode.Success, resPM);
        }
        return r;
    }
}

public class AAAChangeChannel : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAChangeChannel; }

    *handle(object socket, MsgChangeChannel msg) {
        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0)) {
            logger.info("server not ready");
            return MyResponse.create(ECode.ServerNotReady);
        }

        this.logger.info("AAAChangeChannel playerId: %d, (%s,%s) -> (%s,%s), %s", msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2, JSON.stringify(msg.verifyData2));


        // 和 gameScript.changeChannelCheck 一样的实现，但是这里没有办法调用 gameScript
        if (!this.server.scUtils.checkArgs("SSSS", msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2)) {
            return MyResponse.create(ECode.InvalidParam);
        }
        if (!this.server.scUtils.isValidChannelType(msg.channel1) || !this.server.scUtils.isValidChannelType(msg.channel2)) {
            return MyResponse.create(ECode.InvalidChannel);
        }
        if (msg.channel1 !== HermesChannels.uuid || msg.channel2 == HermesChannels.uuid) {
            // 只允许由 uuid 换成其他的
            return MyResponse.create(ECode.Error);
        }

        MyResponse r = yield this.server.aaaSqlUtils.queryAccountYield(msg.channel1, msg.channelUserId1);
        if (r.err != ECode.Success) {
            return r.err;
        }

        SqlTableAccount accountInfo = r.res;
        if (accountInfo == null) {
            return MyResponse.create(ECode.AccountNotExist);
        }

        // 检查一下 playerId 是否匹配。不匹配表示传了别人的 channel/channelUserId
        if (accountInfo.playerId !== msg.playerId) {
            return MyResponse.create(ECode.PlayerIdNotMatch);
        }

        // 登录验证
        r = yield this.aaaScript.verifyAccount(msg.channel2, msg.channelUserId2, msg.verifyData2);
        if (r.err != ECode.Success) {
            return r.err;
        }
        AAAVerifyAccountResult verifyResult2 = r.res;
        var aaaUserInfo2 = this.aaaScript.getUserInfo(msg.channel2, msg.channelUserId2, msg.verifyData2);

        // 需要检查 channel2 & channelUserId2 是否已经存在
        r = yield this.server.aaaSqlUtils.queryAccountYield(msg.channel2, msg.channelUserId2);
        if (r.err != ECode.Success) {
            return r.err;
        }
        SqlTableAccount accountInfo2 = r.res;
        if (verifyResult2.accountMustExist && accountInfo2 == null) {
            return MyResponse.create(ECode.AccountNotExist2);
        }

       var res = new ResChangeChannel {
            channel2Exist = false,
            loginReward = 0,
            userName = null,
        };
        if (accountInfo2 != null) {
            res.channel2Exist = true;
            // 客户端应登录为此账号
        }
        else {
            res.channel2Exist = false;
            r = yield this.server.aaaSqlUtils.changeChannelYield(msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2, JSON.stringify(aaaUserInfo2));
            if (r.err != ECode.Success) {
                return r.err;
            }

            res.userName = aaaUserInfo2.userName;

            // 记日志
            this.server.sqlLog.account_changeChannel(msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);
        }

        return new MyResponse(ECode.Success, res);
    }
}
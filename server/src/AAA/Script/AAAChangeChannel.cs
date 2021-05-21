
using System.Collections;

public class AAAChangeChannel : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAChangeChannel; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgChangeChannel;

        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0)) {
            logger.info("server not ready");
            r.err = ECode.ServerNotReady;
            yield break;
        }

        this.logger.info("AAAChangeChannel playerId: %d, (%s,%s) -> (%s,%s), %s", msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2, JSON.stringify(msg.verifyData2));

        // 和 gameScript.changeChannelCheck 一样的实现，但是这里没有办法调用 gameScript
        if (!this.server.scUtils.checkArgs("SSSS", msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2)) {
            r.err = ECode.InvalidParam;
            yield break;
        }
        if (!this.server.scUtils.isValidChannelType(msg.channel1) || !this.server.scUtils.isValidChannelType(msg.channel2)) {
            r.err = ECode.InvalidChannel;
            yield break;
        }
        if (msg.channel1 != HermesChannels.uuid || msg.channel2 == HermesChannels.uuid) {
            // 只允许由 uuid 换成其他的
            r.err = ECode.Error;
            yield break;
        }

        yield return this.server.aaaSqlUtils.queryAccountYield(msg.channel1, msg.channelUserId1, r);
        if (r.err != ECode.Success) {
            yield break;
        }

        var accountInfo = r.res as SqlTableAccount;
        if (accountInfo == null) {
            r.err = ECode.AccountNotExist;
            yield break;
        }

        // 检查一下 playerId 是否匹配。不匹配表示传了别人的 channel/channelUserId
        if (accountInfo.playerId != msg.playerId) {
            r.err = ECode.PlayerIdNotMatch;
            yield break;
        }

        // 登录验证
        yield return this.aaaScript.verifyAccount(msg.channel2, msg.channelUserId2, msg.verifyData2, r);
        if (r.err != ECode.Success) {
            yield break;
        }
        var verifyResult2 = r.res as AAAVerifyAccountResult;
        var aaaUserInfo2 = this.aaaScript.getUserInfo(msg.channel2, msg.channelUserId2, msg.verifyData2);

        // 需要检查 channel2 & channelUserId2 是否已经存在
        yield return this.server.aaaSqlUtils.queryAccountYield(msg.channel2, msg.channelUserId2, r);
        if (r.err != ECode.Success) {
            yield break;
        }
        var accountInfo2 = r.res as SqlTableAccount;
        if (verifyResult2.accountMustExist && accountInfo2 == null) {
            r.err = ECode.AccountNotExist2;
            yield break;
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
            yield return this.server.aaaSqlUtils.changeChannelYield(msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2, JSON.stringify(aaaUserInfo2), r);
            if (r.err != ECode.Success) {
                yield break;
            }

            res.userName = aaaUserInfo2.userName;

            // 记日志
            this.server.sqlLog.account_changeChannel(msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);
        }

        r.err = ECode.Success;
        r.res = res;
    }
}
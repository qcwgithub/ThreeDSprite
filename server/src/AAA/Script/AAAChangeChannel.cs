
using System.Collections;
using System.Threading.Tasks;

public class AAAChangeChannel : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAAChangeChannel; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        MyResponse r = null;
        var msg = _msg as MsgChangeChannel;

        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0))
        {
            logger.info("server not ready");
            return ECode.ServerNotReady;
        }

        this.logger.info("AAAChangeChannel playerId: %d, (%s,%s) -> (%s,%s), %s", msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2, this.server.JSON.stringify(msg.verifyData2));

        // 和 gameScript.changeChannelCheck 一样的实现，但是这里没有办法调用 gameScript
        if (!this.server.scUtils.checkArgs("SSSS", msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2))
        {
            return ECode.InvalidParam;
        }
        if (!this.server.scUtils.isValidChannelType(msg.channel1) || !this.server.scUtils.isValidChannelType(msg.channel2))
        {
            return ECode.InvalidChannel;
        }
        if (msg.channel1 != HermesChannels.uuid || msg.channel2 == HermesChannels.uuid)
        {
            // 只允许由 uuid 换成其他的
            return ECode.Error;
        }

        r = await this.server.aaaSqlUtils.queryAccountYield(msg.channel1, msg.channelUserId1);
        if (r.err != ECode.Success)
        {
            return r;
        }

        var accountInfo = r.res as SqlTableAccount;
        if (accountInfo == null)
        {
            return ECode.AccountNotExist;
        }

        // 检查一下 playerId 是否匹配。不匹配表示传了别人的 channel/channelUserId
        if (accountInfo.playerId != msg.playerId)
        {
            return ECode.PlayerIdNotMatch;
        }

        // 登录验证
        r = await this.aaaScript.verifyAccount(msg.channel2, msg.channelUserId2, msg.verifyData2);
        if (r.err != ECode.Success)
        {
            return r;
        }
        var verifyResult2 = r.res as AAAVerifyAccountResult;
        var aaaUserInfo2 = this.aaaScript.getUserInfo(msg.channel2, msg.channelUserId2, msg.verifyData2);

        // 需要检查 channel2 & channelUserId2 是否已经存在
        r = await this.server.aaaSqlUtils.queryAccountYield(msg.channel2, msg.channelUserId2);
        if (r.err != ECode.Success)
        {
            return r;
        }
        var accountInfo2 = r.res as SqlTableAccount;
        if (verifyResult2.accountMustExist && accountInfo2 == null)
        {
            return ECode.AccountNotExist2;
        }

        var res = new ResChangeChannel
        {
            channel2Exist = false,
            loginReward = 0,
            userName = null,
        };
        if (accountInfo2 != null)
        {
            res.channel2Exist = true;
            // 客户端应登录为此账号
        }
        else
        {
            res.channel2Exist = false;
            r = await this.server.aaaSqlUtils.changeChannelYield(
                msg.channel1, msg.channelUserId1,
                msg.channel2, msg.channelUserId2,
                this.server.JSON.stringify(aaaUserInfo2));
            if (r.err != ECode.Success)
            {
                return r;
            }

            res.userName = aaaUserInfo2.userName;

            // 记日志
            this.server.sqlLog.account_changeChannel(msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);
        }
        
        return new MyResponse(ECode.Success, res);
    }
}
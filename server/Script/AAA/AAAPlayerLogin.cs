using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

// client login to AAA
public class AAAPlayerLogin : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAAPlayerLogin; } }

    private async Task<MyResponse> queryAccount(string channel, string channelUserId)
    {
        return await this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId);
    }

    private async Task<MyResponse> newAccount(
        string platform, string channel, string channelUserId, string oaid, string imei, string userInfo)
    {
        SqlTableAccount accountInfo = null;

        var playerId = this.aaaData.nextPlayerId++;
        this.logger.Info($"account {channel},{channelUserId} does not exist, new playerId: {playerId}");
        var r = await this.server.aaaSqlUtils.updatePlayerIdYield(this.aaaData.nextPlayerId);
        if (r.err != ECode.Success)
        {
            return r;
        }

        accountInfo = new SqlTableAccount
        {
            platform = platform,
            channel = channel,
            channelUserId = channelUserId,
            playerId = playerId,
            isBan = false,
            unbanTime = 0,
            createTime = this.server.baseScript.getTimeMs(),
            oaid = oaid,
            imei = imei,
            userInfo = userInfo,
        };

        return await this.server.aaaSqlUtils.insertAccountYield(accountInfo);
    }


    // 仅 debug 使用
    // *devAuth(string id) {
    //     // 暂不限制
    //     // if (!this.baseScript.isLocalhost()) {
    //     //     return MyResponse.create(ECode.TestIdIsAllowedOnLocalhost);
    //     // }

    //     MyResponse r = null;
    //     SqlTableAccount accountInfo = null;

    //     if (this.baseScript.isNumber(id)) {
    //         // 纯数字当做 playerId，登录已有账号
    //         var playerId = parseInt(id);

    //         r = r = await this.server.aaaSqlUtils.queryAccountByPlayerIdYield(playerId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }

    //         accountInfo = r.res;
    //         if (accountInfo == null) {
    //             return MyResponse.create(ECode.PlayerNotExist);
    //         }
    //     }
    //     else {
    //         var channel = "test";
    //         var channelUserId = id;
    //         r = r = await this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }

    //         accountInfo = r.res;
    //         if (accountInfo == null) {
    //             r = r = await this.newAccount(channel, channelUserId);
    //             if (r.err != ECode.Success) {
    //                 return r.err;
    //             }
    //             accountInfo = r.res;
    //         }
    //     }

    //     return new MyResponse(ECode.Success, accountInfo);
    // }

    // *verifyLogin_leiting(string channel, string userlId, string token, string game) {
    //     if (!this.baseScript.checkArgs("SSSS", token, game, channel, channelUserId)) {
    //         return MyResponse.create(ECode.InvalidParam);
    //     }

    //     MyResponse r = null;
    //     AAAAccountInfo accountInfo = null;

    //     // 登录验证
    //     r = r = await this.aaaScript.verifyLogin_leiting(token, game, channelType);
    //     if (r.err != ECode.Success) {
    //         this.baseScript.error("verifyLogin(token: %s, game: %s, channelNo: %s) %s", token, game, channelType, r.res.error);
    //         return MyResponse.create(ECode.VerifyLoginFailed);
    //     }

    //     var loginRes = r.res as AAALoginResponseData;
    //     var channel = msg.channel;
    //     var channelUserId = loginRes.sid;

    //     r = r = await this.baseScript.queryDbAccountYield(this.aaaScript.selectAccountSql(channelType, channelUserId));
    //     if (r.err != ECode.Success) {
    //         return r.err;
    //     }

    //     accountInfo = this.aaaScript.decodeAccount(r.res);
    //     if (accountInfo == null) {
    //         r = r = await this.newAccount(channelType, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }
    //         accountInfo = r.res;
    //     }

    //     return new MyResponse(ECode.Success, accountInfo);
    // }

    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgLoginAAA>(_msg);

        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0))
        {
            logger.Info("server not ready");
            //r.err = ECode.ServerNotReady;
            return ECode.ServerNotReady;
        }

        // 检查参数
        if (!this.server.scUtils.isValidChannelType(msg.channel))
        {
            return ECode.InvalidChannel;
        }

        // 检查版本号
        if (msg.platform == "android")
        {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version != this.server.androidVersion))
            {
                return ECode.LowVersion;
            }
        }
        else if (msg.platform == "ios")
        {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version != this.server.iOSVersion))
            {
                return ECode.LowVersion;
            }
        }
        else if (msg.platform == "101")
        { // DESKTOP BROWSER

        }
        else
        {
            return ECode.InvalidPlatform;
        }
        this.logger.InfoFormat("{0} channel:{1}, channelUserId:{2} version:{3} ", this.msgName, msg.channel, msg.channelUserId, msg.version);

        // 验证登录
        var r = await this.aaaScript.verifyAccount(msg.channel, msg.channelUserId, msg.verifyData);
        if (r.err != ECode.Success)
        {
            return r;
        }

        var verifyResult = r.res as AAAVerifyAccountResult;
        var aaaUserInfo = this.aaaScript.getUserInfo(msg.channel, msg.channelUserId, msg.verifyData);

        // 查询已有账号
        r = await this.queryAccount(msg.channel, msg.channelUserId);
        if (r.err != ECode.Success)
        {
            return r;
        }

        var accountInfo = r.res as SqlTableAccount;

        if (accountInfo == null)
        {
            if (verifyResult.accountMustExist)
            {
                return ECode.AccountNotExist2;
            }

            // 创建新账号
            r = await this.newAccount(
                msg.platform, msg.channel, msg.channelUserId, msg.oaid, msg.imei, 
                this.server.JSON.stringify(aaaUserInfo));
            if (r.err != ECode.Success)
            {
                return r;
            }
            accountInfo = r.res as SqlTableAccount;
        }

        // if (msg2.testId) {
        //     r = r = await this.devAuth(msg2.testId);
        // }
        // else {
        //     r = r = await this.formalAuth(msg2);
        // }

        // if (r.err != ECode.Success) {
        //     return r.err;
        // }
        // accountInfo = r.res;

        if (accountInfo.isBan)
        {
            if (this.baseScript.getTimeMs() > accountInfo.unbanTime)
            {
                this.server.aaaSqlUtils.unbanAccount(accountInfo.playerId);
            }
            else
            {
                return ECode.AccountBan;
            }
        }

        // var $p = "player_" + accountInfo.playerId;
        // if (this.baseScript.isLocked($p)) {
        //     return MyResponse.create(ECode.PlayerLock);
        // }

        var player = aaaData.GetPlayerInfo(accountInfo.playerId);
        if (player == null)
        {
            player = new AAAPlayerInfo();
            player.id = accountInfo.playerId;
            player.pmId = 0;
            aaaData.playerInfos.Add(accountInfo.playerId, player);
        }

        // if (player.socket != null && player.socket != socket) {
        //     aaaScript.clearPlayerSocket(player);
        // }

        // if (player.socket != null && player.socket.id != socket.id) {
        //     player.socket.removeAllListeners();
        //     player.socket.disconnect(true);
        //     player.socket = null;
        // }
        // player.socket = socket;

        // this.baseScript.removePending(socket.id);

        // 分配PM
        AAAPlayerManagerInfo pm = null;
        if (player.pmId == 0)
        {
            logger.Info("alloc pm for playerId: " + player.id);

            // 查找人数最少的pm
            foreach (var kv in aaaData.playerManagerInfos)
            {
                var v = kv.Value;
                if (!v.allowNewPlayer)
                    continue;
                if (!v.socket.isConnected())
                    continue;

                if (pm == null || v.playerCount < pm.playerCount)
                    pm = v;
            }

            if (pm != null)
                player.pmId = pm.id;
        }
        else
        {
            pm = aaaData.GetPlayerManagerInfo(player.pmId);
            if (pm == null)
            {
                this.server.logger.Error("playerPM == null, pmId: " + player.pmId);
            }
        }

        if (pm == null)
        {
            this.server.logger.Error("no available pm!");
            return ECode.NoAvailablePlayerManager;
        }

        // create a new token for each login
        var token = DateTime.Now.ToString();
        MsgPreparePlayerLogin pmMsg = new MsgPreparePlayerLogin
        {
            playerId = player.id,
            token = token,
            channel = msg.channel,
            channelUserId = msg.channelUserId,
            userName = aaaUserInfo.userName,
        };
        r = await pm.socket.sendAsync(MsgType.PMPreparePlayerLogin, pmMsg);
        if (r.err != ECode.Success)
        {
            return r;
        }

        var pmLoc = this.server.baseScript.getKnownLoc(pm.id);
        string pmUrl = "";
        if (msg.platform == "ios")
        {
            pmUrl = "wss://" + pmLoc.outDomain + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
        }
        else
        {
            pmUrl = "ws://" + pmLoc.outIp + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
        }

        var pmRes = r.res as ResPreparePlayerLogin;
        var clientRes = new ResLoginAAA
        {
            channel = msg.channel,
            channelUserId = msg.channelUserId,
            playerId = player.id,
            pmId = pm.id,
            pmUrl = pmUrl,
            pmToken = token,
            needUploadProfile = pmRes.needUploadProfile,
        };

        // tell client to login to pm
        return new MyResponse(ECode.Success, clientRes);
    }
}
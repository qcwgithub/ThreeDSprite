using System;
using System.Collections;

// client login to AAA
public class AAAPlayerLogin : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAAPlayerLogin; } }

    private IEnumerator queryAccount(string channel, string channelUserId, MyResponse res)
    {
        yield return this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId, res);
    }

    private IEnumerator newAccount(string platform, string channel, string channelUserId, string oaid, string imei, string userInfo, MyResponse res)
    {
        SqlTableAccount accountInfo = null;

        var playerId = this.aaaData.nextPlayerId++;
        this.logger.info($"account {channel},{channelUserId} does not exist, new playerId: {playerId}");
        yield return this.server.aaaSqlUtils.updatePlayerIdYield(this.aaaData.nextPlayerId, res);
        if (res.err != ECode.Success)
        {
            yield break;
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

        yield return this.server.aaaSqlUtils.insertAccountYield(accountInfo, res);
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

    //         r = yield return this.server.aaaSqlUtils.queryAccountByPlayerIdYield(playerId);
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
    //         r = yield return this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }

    //         accountInfo = r.res;
    //         if (accountInfo == null) {
    //             r = yield return this.newAccount(channel, channelUserId);
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
    //     r = yield return this.aaaScript.verifyLogin_leiting(token, game, channelType);
    //     if (r.err != ECode.Success) {
    //         this.baseScript.error("verifyLogin(token: %s, game: %s, channelNo: %s) %s", token, game, channelType, r.res.error);
    //         return MyResponse.create(ECode.VerifyLoginFailed);
    //     }

    //     var loginRes = r.res as AAALoginResponseData;
    //     var channel = msg.channel;
    //     var channelUserId = loginRes.sid;

    //     r = yield return this.baseScript.queryDbAccountYield(this.aaaScript.selectAccountSql(channelType, channelUserId));
    //     if (r.err != ECode.Success) {
    //         return r.err;
    //     }

    //     accountInfo = this.aaaScript.decodeAccount(r.res);
    //     if (accountInfo == null) {
    //         r = yield return this.newAccount(channelType, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }
    //         accountInfo = r.res;
    //     }

    //     return new MyResponse(ECode.Success, accountInfo);
    // }

    public override IEnumerator handle(object socket, object _msg, MyResponse res)
    {
        var msg = _msg as MsgLoginAAA;

        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0))
        {
            logger.info("server not ready");
            res.err = ECode.ServerNotReady;
            yield break;
        }

        // 检查参数
        if (!this.server.scUtils.isValidChannelType(msg.channel))
        {
            res.err = ECode.InvalidChannel;
            yield break;
        }

        // 检查版本号
        if (msg.platform == "android")
        {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version != this.server.androidVersion))
            {
                res.err = ECode.LowVersion;
                yield break;
            }
        }
        else if (msg.platform == "ios")
        {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version != this.server.iOSVersion))
            {
                res.err = ECode.LowVersion;
                yield break;
            }
        }
        else if (msg.platform == "101")
        { // DESKTOP BROWSER

        }
        else
        {
            res.err = ECode.InvalidPlatform;
            yield break;
        }
        this.logger.info("%s channel:%s, channelUserId:%s version:%s ", this.msgName, msg.channel, msg.channelUserId, msg.version);

        // 验证登录
        yield return this.aaaScript.verifyAccount(msg.channel, msg.channelUserId, msg.verifyData, res);
        if (res.err != ECode.Success)
        {
            yield break;
        }

        var verifyResult = res.res as AAAVerifyAccountResult;
        var aaaUserInfo = this.aaaScript.getUserInfo(msg.channel, msg.channelUserId, msg.verifyData);

        // 查询已有账号
        yield return this.queryAccount(msg.channel, msg.channelUserId, res);
        if (res.err != ECode.Success)
        {
            yield break;
        }

        var accountInfo = res.res as SqlTableAccount;

        if (accountInfo == null)
        {
            if (verifyResult.accountMustExist)
            {
                res.err = ECode.AccountNotExist2;
                yield break;
            }

            // 创建新账号
            yield return this.newAccount(msg.platform, msg.channel, msg.channelUserId, msg.oaid, msg.imei, JSON.stringify(aaaUserInfo), res);
            if (res.err != ECode.Success)
            {
                yield break;
            }
            accountInfo = res.res as SqlTableAccount;
        }

        // if (msg2.testId) {
        //     r = yield return this.devAuth(msg2.testId);
        // }
        // else {
        //     r = yield return this.formalAuth(msg2);
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
                res.err = ECode.AccountBan;
                yield break;
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
            logger.info("alloc pm for playerId: " + player.id);

            // 查找人数最少的pm
            foreach (var kv in aaaData.playerManagerInfos)
            {
                var v = kv.Value;
                if (!v.allowNewPlayer)
                    continue;
                if (!this.server.netProto.isConnected(v.socket))
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
                this.server.baseScript.error("playerPM == null, pmId: " + player.pmId);
            }
        }

        if (pm == null)
        {
            this.server.baseScript.error("no available pm!");
            res.err = ECode.NoAvailablePlayerManager;
            yield break;
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
        yield return this.baseScript.sendYield(pm.socket, MsgType.PMPreparePlayerLogin, pmMsg, res);
        if (res.err != ECode.Success)
        {
            yield break;
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

        var pmRes = res.res as ResPreparePlayerLogin;
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
        res.err = ECode.Success;
        res.res = clientRes;
    }
}
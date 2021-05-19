
// client login to AAA
public class AAAPlayerLogin : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAPlayerLogin; }
    *queryAccount(string channel, string channelUserId) {
        var r = yield this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId);
        if (r.err != ECode.Success) {
            return r.err;
        }

        var accountInfo = r.res;
        return new MyResponse(ECode.Success, accountInfo);
    }

    *newAccount(string platform, string channel, string channelUserId, string oaid, string imei, string userInfo) {
        MyResponse r = null;
        SqlTableAccount accountInfo = null;

        var playerId = this.aaaData.nextPlayerId++;
        this.logger.info(`account "${channel},${channelUserId} does not exist, new playerId: ${playerId}`);
        r = yield this.server.aaaSqlUtils.updatePlayerIdYield(this.aaaData.nextPlayerId);
        if (r.err != ECode.Success) {
            return r.err;
        }

        accountInfo = new SqlTableAccount {
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

        r = yield this.server.aaaSqlUtils.insertAccountYield(accountInfo);
        if (r.err != ECode.Success) {
            return r.err;
        }

        return new MyResponse(ECode.Success, accountInfo);
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

    //         r = yield this.server.aaaSqlUtils.queryAccountByPlayerIdYield(playerId);
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
    //         r = yield this.server.aaaSqlUtils.queryAccountYield(channel, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }

    //         accountInfo = r.res;
    //         if (accountInfo == null) {
    //             r = yield this.newAccount(channel, channelUserId);
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
    //     r = yield this.aaaScript.verifyLogin_leiting(token, game, channelType);
    //     if (r.err != ECode.Success) {
    //         this.baseScript.error("verifyLogin(token: %s, game: %s, channelNo: %s) %s", token, game, channelType, r.res.error);
    //         return MyResponse.create(ECode.VerifyLoginFailed);
    //     }

    //     var loginRes = r.res as AAALoginResponseData;
    //     var channel = msg.channel;
    //     var channelUserId = loginRes.sid;

    //     r = yield this.baseScript.queryDbAccountYield(this.aaaScript.selectAccountSql(channelType, channelUserId));
    //     if (r.err != ECode.Success) {
    //         return r.err;
    //     }

    //     accountInfo = this.aaaScript.decodeAccount(r.res);
    //     if (accountInfo == null) {
    //         r = yield this.newAccount(channelType, channelUserId);
    //         if (r.err != ECode.Success) {
    //             return r.err;
    //         }
    //         accountInfo = r.res;
    //     }

    //     return new MyResponse(ECode.Success, accountInfo);
    // }

    *handle(object socket, MsgLoginAAA msg) {
        var logger = this.logger;
        var aaaData = this.aaaData;
        var aaaScript = this.aaaScript;

        if (!(aaaData.nextPlayerId > 0)) {
            logger.info("server not ready");
            return MyResponse.create(ECode.ServerNotReady);
        }

        // 检查参数
        if (!this.baseScript.checkArgs("SS", msg.channel, msg.channelUserId)) {
            return MyResponse.create(ECode.InvalidParam);
        }

        if (!this.server.scUtils.isValidChannelType(msg.channel)) {
            return MyResponse.create(ECode.InvalidChannel);
        }

        // 检查版本号
        if (msg.platform == "android") {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version !== this.server.androidVersion && msg.version !== "85")) {
                return MyResponse.create(ECode.LowVersion);
            }
        }
        else if (msg.platform == "ios") {
            if (this.server.purpose != Purpose.Test && // 测试版本不检查版本号
                (msg.version !== this.server.iOSVersion && msg.version !== "1.22" && msg.version !== "1.23")) {
                return MyResponse.create(ECode.LowVersion);
            }
        }
        else if (msg.platform == "101") { // DESKTOP BROWSER

        }
        else {
            return MyResponse.create(ECode.InvalidPlatform);
        }
        this.logger.info("%s channel:%s, channelUserId:%s version:%s ", this.msgName, msg.channel, msg.channelUserId, msg.version);

        // 验证登录
        MyResponse r = yield this.aaaScript.verifyAccount(msg.channel, msg.channelUserId, msg.verifyData);
        if (r.err != ECode.Success) {
            return r.err;
        }
        AAAVerifyAccountResult verifyResult = r.res;
        var aaaUserInfo = this.aaaScript.getUserInfo(msg.channel, msg.channelUserId, msg.verifyData);

        // 查询已有账号
        r = yield this.queryAccount(msg.channel, msg.channelUserId);
        if (r.err != ECode.Success) {
            return r.err;
        }
        SqlTableAccount accountInfo = r.res;

        if (accountInfo == null) {
            if (verifyResult.accountMustExist) {
                return MyResponse.create(ECode.AccountNotExist2);
            }

            // 创建新账号
            r = yield this.newAccount(msg.platform, msg.channel, msg.channelUserId, msg.oaid, msg.imei, JSON.stringify(aaaUserInfo));
            if (r.err != ECode.Success) {
                return r.err;
            }
            accountInfo = r.res;
        }

        // if (msg2.testId) {
        //     r = yield this.devAuth(msg2.testId);
        // }
        // else {
        //     r = yield this.formalAuth(msg2);
        // }

        // if (r.err != ECode.Success) {
        //     return r.err;
        // }
        // accountInfo = r.res;

        if (accountInfo.isBan) {
            if (this.baseScript.getTimeMs() > accountInfo.unbanTime) {
                this.server.aaaSqlUtils.unbanAccount(accountInfo.playerId);
            }
            else {
                return MyResponse.create(ECode.AccountBan);
            }
        }

        // var $p = "player_" + accountInfo.playerId;
        // if (this.baseScript.isLocked($p)) {
        //     return MyResponse.create(ECode.PlayerLock);
        // }

        var player = aaaData.playerInfos.get(accountInfo.playerId);
        if (player == null) {
            player = new AAAPlayerInfo();
            player.id = accountInfo.playerId;
            player.pmId = 0;
            aaaData.playerInfos.set(accountInfo.playerId, player);
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
        if (player.pmId == 0) {
            logger.info("alloc pm for playerId: " + player.id);

            // 查找人数最少的pm
            aaaData.playerManagerInfos.forEach((v, k, m) => {
                if (!v.allowNewPlayer) {
                    return;
                }
                if (!this.server.netProto.isConnected(v.socket)) {
                    return;
                }
                if (pm == null || v.playerCount < pm.playerCount) {
                    pm = v;
                }
            });
            if (pm != null) {
                player.pmId = pm.id;
            }
        }
        else {
            pm = aaaData.playerManagerInfos.get(player.pmId);
            if (pm == null) {
                this.server.baseScript.error("playerPM == null, pmId: " + player.pmId);
            }
        }

        if (pm == null) {
            this.server.baseScript.error("no available pm!");
            return MyResponse.create(ECode.NoAvailablePlayerManager);
        }

        // create a new token for each login
        var token = new Date().toString();
        MsgPreparePlayerLogin pmMsg = {
            playerId: player.id,
            token: token,
            channel: msg.channel,
            channelUserId: msg.channelUserId,
            userName: aaaUserInfo.userName,
        };
        r = yield this.baseScript.sendYield(pm.socket, MsgType.PMPreparePlayerLogin, pmMsg);
        if (r.err != ECode.Success) {
            return r;
        }

        var pmLoc = this.server.baseScript.getKnownLoc(pm.id);
        var string pmUrl = "";
        if (msg.platform == "ios") {
            pmUrl = "wss://" + pmLoc.outDomain + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
        }
        else {
            pmUrl = "ws://" + pmLoc.outIp + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
        }

       var pmRes = new ResPreparePlayerLogin r.res;
       var clientRes = new ResLoginAAA {
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
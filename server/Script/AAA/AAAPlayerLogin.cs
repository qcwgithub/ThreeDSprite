using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    // client login to AAA
    public class AAAPlayerLogin : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAAPlayerLogin; } }

        private async Task<MyResponse> newAccount(
            string platform, string channel, string channelUserId, string oaid, string imei, string userInfo)
        {
            SqlTableAccount accountInfo = null;

            var playerId = this.aaaData.nextPlayerId++;
            this.logger.Info($"account {channel},{channelUserId} does not exist, new playerId: {playerId}");
            var r = await this.server.aaaSqlUtils.updatePlayerIdAsync(this.aaaData.nextPlayerId);
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
                unbanTimeS = 0,
                createTimeS = this.server.getTimeS(),
                oaid = oaid,
                imei = imei,
                userInfo = userInfo,
            };

            r = await this.server.aaaSqlUtils.insertAccountAsync(accountInfo);
            if (r.err != ECode.Success)
            {
                return r;
            }
            return new MyResponse(ECode.Success, accountInfo);
        }

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

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLoginAAA>(_msg);

            var logger = this.logger;
            var aaaData = this.aaaData;
            var aaaScript = this.aaaScript;

            if (!(aaaData.nextPlayerId > 0))
            {
                logger.InfoFormat("{0} !(aaaData.nextPlayerId > 0)", this.msgName);
                return ECode.ServerNotReady;
            }

            if (!aaaData.active)
            {
                logger.InfoFormat("{0} !aaaData.active", this.msgName);
                return ECode.ServerNotReady;
            }
            
            if (!aaaData.pmReady)
            {
                logger.InfoFormat("{0} !aaaData.pmReady", this.msgName);
                return ECode.ServerNotReady;
            }

            // 检查参数
            if (!MyChannels.isValidChannel(msg.channel))
            {
                return ECode.InvalidChannel;
            }

            // 检查版本号
            if (msg.platform == "android")
            {
                if (this.server.dataEntry.purpose != Purpose.tds_test && // 测试版本不检查版本号
                    (msg.version != this.server.dataEntry.androidVersion))
                {
                    return ECode.LowVersion;
                }
            }
            else if (msg.platform == "ios")
            {
                if (this.server.dataEntry.purpose != Purpose.tds_test && // 测试版本不检查版本号
                    (msg.version != this.server.dataEntry.iOSVersion))
                {
                    return ECode.LowVersion;
                }
            }
            else if (msg.platform == "pc")
            {

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
            r = await this.server.aaaSqlUtils.queryAccountByChannel(msg.channel, msg.channelUserId);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var resDBQueryAccount = r.res as ResQueryAccount;
            SqlTableAccount accountInfo = null;
            if (resDBQueryAccount.list.Count == 0)
            {
                if (verifyResult.accountMustExist)
                {
                    return ECode.AccountNotExist2;
                }

                // 创建新账号
                r = await this.newAccount(
                    msg.platform, msg.channel, msg.channelUserId, msg.oaid, msg.imei,
                    JsonUtils.stringify(aaaUserInfo));
                if (r.err != ECode.Success)
                {
                    return r;
                }
                accountInfo = r.res as SqlTableAccount;
            }
            else
            {
                accountInfo = resDBQueryAccount.list[0];
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

            // if (accountInfo.isBan)
            // {
            //     if (this.server.getTimeMs() > accountInfo.unbanTime)
            //     {
            //         this.server.aaaSqlUtils.unbanAccount(accountInfo.playerId);
            //     }
            //     else
            //     {
            //         return ECode.AccountBan;
            //     }
            // }

            // var $p = "player_" + accountInfo.playerId;
            // if (this.baseScript.isLocked($p)) {
            //     return MyResponse.create(ECode.PlayerLock);
            // }

            var player = aaaData.GetPlayer(accountInfo.playerId);
            if (player == null)
            {
                player = new AAAPlayer();
                player.playerId = accountInfo.playerId;
                player.pmId = 0;
                aaaData.playerDict.Add(accountInfo.playerId, player);
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
            AAAPlayerManager pm = null;
            if (player.pmId == 0)
            {
                logger.Info("alloc pm for playerId: " + player.playerId);

                // 查找人数最少的pm
                foreach (var kv in aaaData.playerManagerDict)
                {
                    var v = kv.Value;
                    if (!v.allowNewPlayer)
                        continue;
                    if (!this.server.tcpClientScript.isServerConnected(v.pmId))
                        continue;

                    if (pm == null || v.playerCount < pm.playerCount)
                        pm = v;
                }

                if (pm != null)
                    player.pmId = pm.pmId;
            }
            else
            {
                pm = aaaData.GetPlayerManager(player.pmId);
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
                playerId = player.playerId,
                token = token,
                channel = msg.channel,
                channelUserId = msg.channelUserId,
                userName = aaaUserInfo.userName,
            };
            r = await this.server.tcpClientScript.sendToServerAsync(pm.pmId, MsgType.PMPreparePlayerLogin, pmMsg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var pmLoc = this.server.getKnownLoc(pm.pmId);
            // string pmUrl = "";
            // if (msg.platform == "ios")
            // {
            //     pmUrl = "wss://" + pmLoc.outDomain + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
            // }
            // else
            // {
            //     pmUrl = "ws://" + pmLoc.outIp + ":" + pmLoc.port + "?sign=" + ServerConst.CLIENT_SIGN;
            // }

            var pmRes = r.res as ResPreparePlayerLogin;
            var clientRes = new ResLoginAAA
            {
                channel = msg.channel,
                channelUserId = msg.channelUserId,
                playerId = player.playerId,
                pmId = pm.pmId,
                // pmUrl = pmUrl,
                pmIp = pmLoc.outIp,
                pmPort = pmLoc.outPort,
                pmToken = token,
                needUploadProfile = pmRes.needUploadProfile,
            };

            // tell client to login to pm
            return new MyResponse(ECode.Success, clientRes);
        }
    }
}
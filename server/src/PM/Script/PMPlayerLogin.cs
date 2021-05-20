
public class PMPlayerLogin : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMPlayerLogin; } }
    *handle(object socket, MsgLoginPM msg)
    {
        // this.logger.info("PMPlayerLogin playerId: " + msg.playerId);

        if (!this.baseScript.checkArgs("IS", msg.playerId, msg.token))
        {
            // 客户端遇到这个错误会转连AAA
            r.err = ECode.InvalidParam;
            yield break;
        }

        var player = this.pmData.GetPlayerInfo(msg.playerId);
        if (player == null)
        {
            // 客户端遇到这个错误会转连AAA
            r.err = ECode.ShouldLoginAAA;
            yield break;
        }

        if (msg.token !== player.token)
        {
            // 客户端遇到这个错误会转连AAA
            r.err = ECode.InvalidToken;
            yield break;
        }

        this.logger.info("%s playerId: %d, preCount: %d", this.msgName, player.id, this.pmData.playerInfos.size);

        // if (this.baseScript.isMessageListenerAdded(socket)) {
        //     this.logger.info("PMPlayerLogin playerId: " + msg2.playerId + " MessageListenerAdded");
        //     return MyResponse.create(ECode.MessageListenerAdded);
        // }

        // 除了 PMPreparePlayerLogin，这里也需要对 oldSocket 做检测，因为客户端重连时不会经过 PMPreparePlayerLogin
        var oldSocket = player.socket;
        if (oldSocket != null)
        {
            // 情况1 同一个客户端意外地登录2次
            // 情况2 客户端A已经登录，B再登录
            this.logger.info("2 playerId: %d, ECode.OldSocket oldSocket: %s", player.id, this.server.netProto.getSocketId(oldSocket));
            var resMisc = new ResMisc
            {
                oldSocketTimestamp = this.server.netProto.getSocketClientTimestamp(oldSocket),
            };
            return new MyResponse(ECode.OldSocket, resMisc);
        }

        var oldPlayer = this.server.netProto.getPlayer(socket);
        if (oldPlayer != null)
        {
            // 情况1 同一个客户端意外地登录2次
            // 情况2 客户端A已经登录，B再登录
            this.baseScript.error("playerId %d, ECode.OldPlayer %d", player.id, oldPlayer.id);
            r.err = ECode.OldPlayer;
            yield break;
        }

        if (player.destroyTimer != null)
        {
            this.pmScript.clearDestroyTimer(player);
        }

        this.server.netProto.bindPlayerAndSocket(player, socket, msg.timestamp);
        // this.baseScript.removePending(this.server.networkHelper.getSocketId(socket));

        if (!msg.isReconnect)
        {
            player.profile.totalLoginTimes++;
            player.profileChanged(PMProfileType.totalLoginTimes);
        }
        // 有可能首次登录就有账号了，此时需要标记为可领取登录奖励
        if (player.profile.loginReward == 0 && player.channel != HermesChannels.uuid && 
            this.server.scUtils.isValidChannelType(player.channel))
        {
            player.profile.loginReward = 1;
            player.profileChanged(PMProfileType.loginReward);
        }

        // this.server.pmScript.onOnline_calcTimeRelative(player);
        this.sqlLog.player_login(player);

        // 发送玩家数据
        var locAAA = this.baseData.knownLocs.get(ServerConst.AAA_ID);
        var res = new ResLoginPM
        {
            id = player.id,
            keepSyncProfile = true,
            profile = (msg.isReconnect ? null : player.profile), // 重连不用发送 profile，省流量，同时客户端在重连时并不需要使用
            timeMs = this.server.gameScript.getTime(),
            timezoneOffset = this.server.baseData.timezoneOffset,

            // 几个时间相关的，在登录时发给客户端
            offlineBonusTime = player.profile.offlineBonus.time,
            totalGameTimeMs = player.profile.totalGameTimeMs,
            totalLoginTimes = player.profile.totalLoginTimes,
            diamond = player.profile.diamond,
            badge = player.profile.badge,

            updateProfile = (msg.isReconnect ? this.pmScript.createUpdateProfile(player) : null),
            ltProducts = this.pmData.iapConfig.platformInfo.leiting,
            payNotifyUri = "http://" + locAAA.outIp + ":" + ServerConst.AAA_LT_NOTIFY_PORT,
            script = null,
        };

        return new MyResponse(ECode.Success, res);
    }
}
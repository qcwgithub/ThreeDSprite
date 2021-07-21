using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMPlayerLogin : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMPlayerLogin; } }
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLoginPM>(_msg);
            // this.logger.info("PMPlayerLogin playerId: " + msg.playerId);

            if (msg.playerId <= 0 || msg.token == null)
            {
                // 客户端遇到这个错误会转连AAA
                return ECode.InvalidParam.toTask();
            }

            var player = this.data.GetPlayerInfo(msg.playerId);
            if (player == null)
            {
                // 客户端遇到这个错误会转连AAA
                return ECode.ShouldLoginAAA.toTask();
            }

            if (msg.token != player.token)
            {
                // 客户端遇到这个错误会转连AAA
                return ECode.InvalidToken.toTask();
            }

            this.logger.InfoFormat("{0} playerId: {1}, preCount: {2}", this.msgName, player.playerId, this.data.playerDict.Count);

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
                this.logger.InfoFormat("2 playerId: {0}, ECode.OldSocket oldSocket: {1}", player.playerId, oldSocket.getSocketId());
                var resMisc = new ResMisc
                {
                    oldSocketTimestamp = this.server.tcpClientScript.getClientTimestamp(oldSocket),
                };
                return new MyResponse(ECode.OldSocket, resMisc).toTask();
            }

            var oldPlayer = this.getPlayer(socket);
            if (oldPlayer != null)
            {
                // 情况1 同一个客户端意外地登录2次
                // 情况2 客户端A已经登录，B再登录
                this.logger.ErrorFormat("playerId {0}, ECode.OldPlayer {1}", player.playerId, oldPlayer.playerId);
                return ECode.OldPlayer.toTask();
            }

            if (player.destroyTimer > 0)
            {
                this.pmScript.clearDestroyTimer(player);
            }

            this.server.tcpClientScript.bindPlayer(socket, player, msg.timestamp);
            // this.baseScript.removePending(this.server.networkHelper.getSocketId(socket));

            if (!msg.isReconnect)
            {
                // player.profile.totalLoginTimes++;
                // player.profileChanged(PMProfileType.totalLoginTimes);
            }
            // 有可能首次登录就有账号了，此时需要标记为可领取登录奖励
            // if (player.profile.loginReward == 0 && player.channel != MyChannels.uuid && 
            //     this.server.scUtils.isValidChannelType(player.channel))
            // {
            //     player.profile.loginReward = 1;
            //     player.profileChanged(PMProfileType.loginReward);
            // }

            // this.server.pmScript.onOnline_calcTimeRelative(player);
            this.sqlLog.player_login(player);

            // 发送玩家数据
            var locAAA = this.server.getKnownLoc(ServerConst.AAA_ID);
            var res = new ResLoginPM
            {
                playerId = player.playerId,
                keepSyncProfile = true,
                // profile = (msg.isReconnect ? null : player.profile), // 重连不用发送 profile，省流量，同时客户端在重连时并不需要使用
                timeMs = this.server.gameScript.getTimeS(),
                timezoneOffset = this.server.dataEntry.timezoneOffset,

                // 几个时间相关的，在登录时发给客户端
                // offlineBonusTime = player.profile.offlineBonus.time,
                // totalGameTimeMs = player.profile.totalGameTimeMs,
                // totalLoginTimes = player.profile.totalLoginTimes,
                // diamond = player.profile.diamond,
                // badge = player.profile.badge,

                // updateProfile = (msg.isReconnect ? this.pmScript.createUpdateProfile(player) : null),
                // ltProducts = this.pmData.iapConfig.platformInfo.leiting,
                payNotifyUri = "http://" + locAAA.outIp + ":" + ServerConst.AAA_LT_NOTIFY_PORT,
            };

            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
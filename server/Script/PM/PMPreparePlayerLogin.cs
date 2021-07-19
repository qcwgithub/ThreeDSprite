
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMPreparePlayerLogin : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMPreparePlayerLogin; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgPreparePlayerLogin>(_msg);
            var data = this.data;
            var script = this.pmScript;
            var logger = this.logger;

            this.logger.InfoFormat("{0} playerId: {1}", this.msgName, msg.playerId);

            if (!data.aaaReady)
            {
                logger.InfoFormat("{0} !data.aaaReady", this.msgName);
                return ECode.ServerNotReady;
            }

            if (!data.allowClientConnect)
            {
                logger.InfoFormat("{0} !data.allowClientConnect", this.msgName);
                return ECode.ServerNotReady;
            }

            var player = data.GetPlayerInfo(msg.playerId);
            if (player != null)
            {
                var oldSocket = player.socket;
                if (oldSocket != null)
                {
                    // 情况1 同一个客户端意外地登录2次
                    // 情况2 客户端A已经登录，B再登录
                    this.logger.InfoFormat("1 playerId: {0}, ECode.OldSocket oldSocket: {1}", player.id, oldSocket.getSocketId());

                    var resMisc = new ResMisc
                    {
                        oldSocketTimestamp = this.server.tcpClientScript.getClientTimestamp(oldSocket),
                    };
                    return new MyResponse(ECode.OldSocket, resMisc);
                }
            }

            if (player == null)
            {
                var r = await this.pmSqlUtils.queryPlayerAsync(msg.playerId);
                if (r.err != ECode.Success)
                {
                    return r;
                }

                var resPlayers = r.res as ResQueryPlayer;
                if (resPlayers.list.Count == 0)
                {
                    logger.Info($"player {msg.playerId} not exist, create a new one!");
                    // player not exist, create player now!
                    player = this.server.pmScriptCreateNewPlayer.newPlayer(msg.playerId, msg.channel, msg.channelUserId, msg.userName);

                    // insert to database
                    r = await this.pmSqlUtils.insertPlayerYield(player);
                    if (r.err != ECode.Success)
                    {
                        return r;
                    }
                }
                else
                {
                    // decode playerInfo
                    var sqlTablePlayer = resPlayers.list[0];
                    player = script.decodePlayer(sqlTablePlayer);
                }

                //// runtime 初始化
                for (int i = 0; i < (int)PMProfileType.Count; i++)
                {
                    player.dataChanged.Add(0);
                    player.dataLast.Add(0);
                }
                data.playerInfos.Add(player.id, player);
            }

            if (player.lastProfile == null)
            {
                // 有值就不能再赋值了，不然玩家上线下线就错了
                player.lastProfile = this.server.pmPlayerToSqlTablePlayer.Convert(player);
            }

            player.channel = msg.channel;
            player.channelUserId = msg.channelUserId;
            player.token = msg.token;
            script.setDestroyTimer(player, this.msgName);
            if (player.saveTimer == 0)
            {
                this.pmScript.setSaveTimer(player);
            }

            var res = new ResPreparePlayerLogin
            {
                needUploadProfile = false,
            };

            return new MyResponse(ECode.Success, res);
        }
    }
}
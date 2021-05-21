
using System.Collections;

public class PMPreparePlayerLogin : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMPreparePlayerLogin; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgPreparePlayerLogin;
        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.info("%s playerId: %d", this.msgName, msg.playerId);

        var player = data.GetPlayerInfo(msg.playerId);
        if (player != null)
        {
            var oldSocket = player.socket;
            if (oldSocket != null)
            {
                // 情况1 同一个客户端意外地登录2次
                // 情况2 客户端A已经登录，B再登录
                this.logger.info("1 playerId: %d, ECode.OldSocket oldSocket: %s", player.id, this.server.netProto.getSocketId(oldSocket));

                var resMisc = new ResMisc
                {
                    oldSocketTimestamp = this.server.netProto.getSocketClientTimestamp(oldSocket),
                };
                r.err = ECode.OldSocket;
                r.res = resMisc;
                yield break;
            }
        }

        if (player == null)
        {
            yield return this.pmSqlUtils.selectPlayerYield(msg.playerId, r);
            if (r.err != ECode.Success)
            {
                yield break;
            }

            if (r.res.length == 0)
            {
                logger.info($"player {msg.playerId} not exist, create a new one!");
                // player not exist, create player now!
                player = this.server.pmScriptCreateNewPlayer.newPlayer(msg.playerId, msg.channel, msg.channelUserId, msg.userName);

                // insert to database
                yield return this.pmSqlUtils.insertPlayerYield(player, r);
                if (r.err != ECode.Success)
                {
                    yield break;
                }
            }
            else
            {
                // decode playerInfo
                player = script.decodePlayer((r.res as SqlTablePlayer[])[0]);
            }

            //// runtime 初始化
            player.server = this.server;
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
            player.lastProfile = this.server.pmPlayerToSqlTablePlayer.convert(player);
        }

        player.channel = msg.channel;
        player.channelUserId = msg.channelUserId;
        player.token = msg.token;
        script.setDestroyTimer(player, "PMPreparePlayerLogin");
        if (player.saveTimer == -1)
        {
            this.pmScript.setSaveTimer(player);
        }

        var res = new ResPreparePlayerLogin
        {
            needUploadProfile = false,
        };
        r.err = ECode.Success;
        r.res = res;
    }
}
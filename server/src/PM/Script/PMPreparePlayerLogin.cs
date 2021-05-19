
public class PMPreparePlayerLogin : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPreparePlayerLogin; } }

    *handle(object socket, MsgPreparePlayerLogin msg) {
        var data = this.pmData;
        var script = this.pmScript;
        var logger = this.logger;

        this.logger.info("%s playerId: %d", this.msgName, msg.playerId);

        var player = data.playerInfos.get(msg.playerId);
        if (player != null) {
            var oldSocket = player.socket;
            if (oldSocket != null) {
                // 情况1 同一个客户端意外地登录2次
                // 情况2 客户端A已经登录，B再登录
                this.logger.info("1 playerId: %d, ECode.OldSocket oldSocket: %s", player.id, this.server.netProto.getSocketId(oldSocket));

               var resMisc = new ResMisc {
                    oldSocketTimestamp: this.server.netProto.getSocketClientTimestamp(oldSocket),
                };
                return new MyResponse(ECode.OldSocket, resMisc);
            }
        }

        if (player == null) {
            MyResponse r = yield this.pmSqlUtils.selectPlayerYield(msg.playerId);
            if (r.err != ECode.Success) {
                return r.err;
            }

            if (r.res.length == 0) {
                logger.info(`player ${msg.playerId} not exist, create a new one!`);
                // player not exist, create player now!
                player = this.server.pmScriptCreateNewPlayer.newPlayer(msg.playerId, msg.channel, msg.channelUserId, msg.userName);

                // insert to database
                r = yield this.pmSqlUtils.insertPlayerYield(player);
                if (r.err != ECode.Success) {
                    return r.err;
                }
            }
            else {
                // decode playerInfo
                player = script.decodePlayer(r.res[0]);
            }

            //// runtime 初始化
            player.server = this.server;
            for (int i = 0; i < PMProfileType.Count; i++) {
                player.dataChanged.push(0);
                player.dataLast.push(0);
            }
            data.playerInfos.set(player.id, player);
            this.server.townScript.init2(player);
        }

        if (player.lastProfile == null) { // 有值就不能再赋值了，不然玩家上线下线就错了
            player.lastProfile = this.server.pmPlayerToSqlTablePlayer.convert(player);
        }

        player.channel = msg.channel;
        player.channelUserId = msg.channelUserId;
        player.token = msg.token;
        script.setDestroyTimer(player, "PMPreparePlayerLogin");
        if (player.saveTimer == null) {
            this.pmScript.setSaveTimer(player);
        }

       var res = new ResPreparePlayerLogin {
            needUploadProfile: this.pmScript.allowUploadProfile(player),
        };
        return new MyResponse(ECode.Success, res);
    }
}
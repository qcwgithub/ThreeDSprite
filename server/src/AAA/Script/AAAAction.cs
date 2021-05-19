
public class AAAAction : AAAHandler {
    public override MsgType msgType { get { return MsgType.ServerAction; }

    *handle(object socket, msg: MsgAAAAction) {
        this.logger.info("%s", this.msgName);
        var aaaData = this.server.aaaData;

        if (msg.active != undefined) {
            aaaData.active = msg.active;
        }

        if (msg.pmPlayerRunScript) {
            for (int i = 0; i < msg.pmPlayerRunScript.playerIds.length; i++) {
                var playerId = msg.pmPlayerRunScript.playerIds[i];
                var player = aaaData.playerInfos.get(playerId);
                if (player == null) {
                    this.logger.info("%s playerRunScript player==null, playerId: %d", this.msgName, playerId);
                    continue;
                }
                var pm = aaaData.playerManagerInfos.get(player.pmId);
                if (pm == null) {
                    this.logger.info("%s playerRunScript pm==null, playerId: %d, pmId: %d", this.msgName, playerId, player.pmId);
                    continue;
                }
                MsgPMAction msgAction = {
                    playerRunScript: {
                        playerIds: [playerId],
                        script: msg.pmPlayerRunScript.script,
                    }
                };
                yield this.server.baseScript.sendYield(pm.socket, MsgType.ServerAction, msgAction);
                yield this.server.baseScript.waitYield(10);
            }
        }

        if (msg.destroyAll) {
            while (true) {
                this.logger.info("%s destroyAllPlayers left %d", this.msgName, aaaData.playerInfos.size);
                if (aaaData.playerInfos.size == 0) {
                    break;
                }
                var playerId = aaaData.playerInfos.keys().next().value;
                MsgDestroyPlayer msgDestroy = { playerId: playerId, place: this.msgName };
                this.server.baseScript.sendToSelf(MsgType.AAADestroyPlayer, msgDestroy);
                yield this.server.baseScript.waitYield(10);
            }
        }

        if (msg.destroyPlayerIds != undefined) {
            for (int i = 0; i < msg.destroyPlayerIds.length; i++) {
                this.logger.info("%s destroyPlayerIds left %d", this.msgName, msg.destroyPlayerIds.length - i);
                var playerId = msg.destroyPlayerIds[i];
                if (!aaaData.playerInfos.has(playerId)) {
                    continue;
                }
                MsgDestroyPlayer msgDestroy = { playerId: playerId, place: this.msgName };
                this.server.baseScript.sendToSelf(MsgType.AAADestroyPlayer, msgDestroy);
                yield this.server.baseScript.waitYield(10);
            }
        }

        return MyResponse.create(ECode.Success);
    }
}

// 1秒进来一次
public class PMKeepAliveToAAA : PMHandler {
    public override MsgType msgType { get { return MsgType.PMKeepAliveToAAA; } }

    *handle(object socket, object msg/* no use */) {
        PMData pmData = this.pmData;
        var alive = this.pmData.alive;

        var s = pmData.aaaSocket;
        if (!this.server.netProto.isConnected(s)) {
            alive.count = 10;
            return MyResponse.create(ECode.Success);
        }

        if (alive.first || alive.requirePlayerList) {

        }
        else {
            // 保持连接的情况下，10秒一次
            alive.count++;
            if (alive.count < 10) {
                return MyResponse.create(ECode.Success);
            }
        }

        alive.count = 0;
        alive.first = false;

        var int[] playerList = null;
        if (alive.requirePlayerList) {
            // 如果刚连上，汇报一下当前服的所有玩家ID
            this.logger.info("alive.requirePlayerList = true");
            alive.requirePlayerList = false;
            playerList = [];
            pmData.playerInfos.forEach((v, k, m) => {
                playerList.push(k);
            });
        }

        MsgPMAlive msgAlive = {
            id: this.baseData.id,
            playerCount: pmData.playerInfos.size,
            loc: this.baseScript.myLoc(),
            playerList: playerList,
            allowNewPlayer: pmData.allowNewPlayer,
        };
        MyResponse r = yield this.baseScript.sendYield(s, MsgType.AAAOnPMAlive, msgAlive);

        if (r.err != ECode.Success) {
            this.baseScript.error("PMKeepAliveToAAA error: " + r.err);
        }
        else {
            this.pmData.aaaReady = true;
            if (r.res && r.res.requirePlayerList) {
                alive.requirePlayerList = true;
            }
        }

        return MyResponse.create(ECode.Success);
    }
}
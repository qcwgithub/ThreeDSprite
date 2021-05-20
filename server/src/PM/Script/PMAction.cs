
// 运维，GM功能
using System.Collections;

public class PMAction : PMHandler
{
    public override MsgType msgType { get { return MsgType.ServerAction; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgPMAction;
        this.logger.info("%s", this.msgName);
        PMData pmData = this.pmData;

        if (msg.allowNewPlayer != undefined && pmData.allowNewPlayer != msg.allowNewPlayer)
        {
            pmData.allowNewPlayer = msg.allowNewPlayer;
            this.logger.info("allowNewPlayer -> " + msg.allowNewPlayer);

            MsgPMAlive msgAlive = new MsgPMAlive
            {
                id = this.baseData.id,
                playerCount = pmData.playerInfos.Count,
                loc = this.baseScript.myLoc(),
                playerList = null,
                allowNewPlayer = pmData.allowNewPlayer,
            };
            yield return this.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAAOnPMAlive, msgAlive, r);
        }

        if (msg.allowClientConnect != undefined && pmData.allowClientConnect != msg.allowClientConnect)
        {
            pmData.allowClientConnect = msg.allowClientConnect;
            this.logger.info("allowClientConnect -> " + msg.allowClientConnect);
        }
        if (msg.playerDestroyTimeoutS != undefined && pmData.playerDestroyTimeoutS != msg.playerDestroyTimeoutS)
        {
            pmData.playerDestroyTimeoutS = msg.playerDestroyTimeoutS;
            this.logger.info("playerDestroyTimeoutS -> " + msg.playerDestroyTimeoutS);
        }
        if (msg.playerSCSaveIntervalS != undefined && pmData.playerSCSaveIntervalS != msg.playerSCSaveIntervalS)
        {
            pmData.playerSCSaveIntervalS = msg.playerSCSaveIntervalS;
            this.logger.info("playerSCSaveIntervalS -> " + msg.playerSCSaveIntervalS);
        }

        // if (msg.playerRunScript) {
        //     var script = msg.playerRunScript.script;
        //     var S = "[function(server,player){";
        //     var E = "}]";
        //     if (!script.startsWith(S)) {
        //         script = S + script + E;
        //         // this.logger.warn("%s auto wrap %s...%s", this.msgName, S, E);
        //     }

        // var fun: (server: Server, PMPlayerInfo player) => void = eval(script)[0];
        // for (int i = 0; i < msg.playerRunScript.playerIds.length; i++) {
        //     var playerId = msg.playerRunScript.playerIds[i];
        //     var player = pmData.playerInfos.get(playerId);
        //     if (player == null) {
        //         this.logger.info("%s playerRunScript player==null, playerId: %d", this.msgName, playerId);
        //         continue;
        //     }
        //     fun(this.server, player);
        // }
        // }

        if (msg.destroyAll)
        {
            while (true)
            {
                this.logger.info("%s destroyAllPlayers left %d", this.msgName, pmData.playerInfos.Count);
                if (pmData.playerInfos.Count == 0)
                {
                    break;
                }
                int playerId = 0;
                foreach (var kv in pmData.playerInfos)
                {
                    playerId = kv.Key;
                    break;
                }

                MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                yield return this.server.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAADestroyPlayer, msgDestroy, r);
                yield return this.server.baseScript.waitYield(10);
            }
        }

        if (msg.destroyPlayerIds != null)
        {
            for (int i = 0; i < msg.destroyPlayerIds.Count; i++)
            {
                this.logger.info("%s destroyPlayerIds left %d", this.msgName, msg.destroyPlayerIds.Count - i);
                var playerId = msg.destroyPlayerIds[i];
                if (!pmData.playerInfos.ContainsKey(playerId))
                {
                    continue;
                }
                MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                yield return this.server.baseScript.sendYield(this.pmData.aaaSocket, MsgType.AAADestroyPlayer, msgDestroy, r);
                yield return this.server.baseScript.waitYield(10);
            }
        }

        r.err = ECode.Success;
        yield break;
    }
}

// 运维，GM功能
using System.Collections;
using System.Threading.Tasks;

public class PMAction : PMHandler
{
    public override MsgType msgType { get { return MsgType.ServerAction; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgPMAction>(_msg);
        this.logger.info("%s", this.msgName);
        PMData pmData = this.pmData;

        if (msg.allowNewPlayer != null)
        {
            pmData.allowNewPlayer = (msg.allowNewPlayer == "true");
            this.logger.info("allowNewPlayer -> " + msg.allowNewPlayer);

            var msgAlive = new MsgPMAlive
            {
                id = this.baseData.id,
                playerCount = pmData.playerInfos.Count,
                loc = this.baseScript.myLoc(),
                playerList = null,
                allowNewPlayer = pmData.allowNewPlayer,
            };
            await this.pmData.aaaSocket.sendAsync(MsgType.AAAOnPMAlive, msgAlive);
        }

        if (msg.allowClientConnect != null)
        {
            pmData.allowClientConnect = msg.allowClientConnect == "true";
            this.logger.info("allowClientConnect -> " + msg.allowClientConnect);
        }
        if (msg.playerDestroyTimeoutS != null)
        {
            pmData.playerDestroyTimeoutS = int.Parse(msg.playerDestroyTimeoutS);
            this.logger.info("playerDestroyTimeoutS -> " + msg.playerDestroyTimeoutS);
        }
        if (msg.playerSCSaveIntervalS != null)
        {
            pmData.playerSCSaveIntervalS = int.Parse(msg.playerSCSaveIntervalS);
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

        if (msg.destroyAll == "true")
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
                await this.pmData.aaaSocket.sendAsync(MsgType.AAADestroyPlayer, msgDestroy);
                await this.server.baseScript.waitAsync(10);
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
                await this.pmData.aaaSocket.sendAsync(MsgType.AAADestroyPlayer, msgDestroy);
                await this.server.baseScript.waitAsync(10);
            }
        }

        return ECode.Success;
    }
}
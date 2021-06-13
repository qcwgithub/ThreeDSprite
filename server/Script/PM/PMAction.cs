
// 运维，GM功能
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMAction : PMHandler
    {
        public override MsgType msgType { get { return MsgType.ServerAction; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgPMAction>(_msg);
            this.logger.Info(this.msgName);
            PMData pmData = this.data;

            if (msg.allowNewPlayer != null)
            {
                pmData.allowNewPlayer = (msg.allowNewPlayer == "true");
                this.logger.Info("allowNewPlayer -> " + msg.allowNewPlayer);

                var msgAlive = new MsgPMAlive
                {
                    id = this.baseData.id,
                    playerCount = pmData.playerInfos.Count,
                    loc = this.server.myLoc(),
                    playerList = null,
                    allowNewPlayer = pmData.allowNewPlayer,
                };
                await this.server.tcpClientScript.sendToServerAsync(ServerConst.AAA_ID, MsgType.AAAOnPMAlive, msgAlive);
            }

            if (msg.allowClientConnect != null)
            {
                pmData.allowClientConnect = msg.allowClientConnect == "true";
                this.logger.Info("allowClientConnect -> " + msg.allowClientConnect);
            }
            if (msg.playerDestroyTimeoutS != null)
            {
                pmData.playerDestroyTimeoutS = int.Parse(msg.playerDestroyTimeoutS);
                this.logger.Info("playerDestroyTimeoutS -> " + msg.playerDestroyTimeoutS);
            }
            if (msg.playerSCSaveIntervalS != null)
            {
                pmData.playerSCSaveIntervalS = int.Parse(msg.playerSCSaveIntervalS);
                this.logger.Info("playerSCSaveIntervalS -> " + msg.playerSCSaveIntervalS);
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
                    this.logger.InfoFormat("{0} destroyAllPlayers left {1}", this.msgName, pmData.playerInfos.Count);
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
                    await this.server.tcpClientScript.sendToServerAsync(ServerConst.AAA_ID, MsgType.AAADestroyPlayer, msgDestroy);
                    await this.server.waitAsync(10);
                }
            }

            if (msg.destroyPlayerIds != null)
            {
                for (int i = 0; i < msg.destroyPlayerIds.Count; i++)
                {
                    this.logger.InfoFormat("{0} destroyPlayerIds left {1}", this.msgName, msg.destroyPlayerIds.Count - i);
                    var playerId = msg.destroyPlayerIds[i];
                    if (!pmData.playerInfos.ContainsKey(playerId))
                    {
                        continue;
                    }
                    MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                    await this.server.tcpClientScript.sendToServerAsync(ServerConst.AAA_ID, MsgType.AAADestroyPlayer, msgDestroy);
                    await this.server.waitAsync(10);
                }
            }

            return ECode.Success;
        }
    }
}
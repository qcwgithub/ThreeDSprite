using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAAction : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.ServerAction; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.server.logger.Info(this.msgName);

            var msg = this.server.castObject<MsgAAAAction>(_msg);

            this.logger.Info(this.msgName);
            var aaaData = this.server.aaaData;

            if (msg.active != null)
            {
                aaaData.active = msg.active == "true";
                this.logger.Info("aaaData.active -> " + aaaData.active);
            }

            if (msg.pmPlayerRunScript != null)
            {
                for (int i = 0; i < msg.pmPlayerRunScript.playerIds.Count; i++)
                {
                    var playerId = msg.pmPlayerRunScript.playerIds[i];
                    var player = aaaData.GetPlayerInfo(playerId);
                    if (player == null)
                    {
                        this.logger.InfoFormat("{0} playerRunScript player==null, playerId: {1}", this.msgName, playerId);
                        continue;
                    }
                    var pm = aaaData.GetPlayerManagerInfo(player.pmId);
                    if (pm == null)
                    {
                        this.logger.InfoFormat("{0} playerRunScript pm==null, playerId: {1}, pmId: {2}", this.msgName, playerId, player.pmId);
                        continue;
                    }
                    MsgPMAction msgAction = new MsgPMAction
                    {
                        playerRunScript = new _PRS
                        {
                            playerIds = new List<int> { playerId },
                            script = msg.pmPlayerRunScript.script,
                        }
                    };
                    await this.server.tcpClientScript.sendToServerAsync(pm.id, MsgType.ServerAction, msgAction);
                    await this.server.waitAsync(10);
                }
            }

            if (msg.destroyAll == "true")
            {
                while (true)
                {
                    this.logger.InfoFormat("{0} destroyAllPlayers left {1}", this.msgName, aaaData.playerInfos.Count);
                    if (aaaData.playerInfos.Count == 0)
                    {
                        break;
                    }

                    int playerId = 0;
                    foreach (var kv in aaaData.playerInfos)
                    {
                        playerId = kv.Key;
                        break;
                    }

                    var msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                    this.server.proxyDispatch(null, MsgType.AAADestroyPlayer, msgDestroy, null);
                    await this.server.waitAsync(10);
                }
            }

            if (msg.destroyPlayerIds != null)
            {
                for (int i = 0; i < msg.destroyPlayerIds.Count; i++)
                {
                    this.logger.InfoFormat("{0} destroyPlayerIds left {1}", this.msgName, msg.destroyPlayerIds.Count - i);
                    var playerId = msg.destroyPlayerIds[i];
                    if (!aaaData.playerInfos.ContainsKey(playerId))
                    {
                        continue;
                    }
                    var msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                    this.server.proxyDispatch(null, MsgType.AAADestroyPlayer, msgDestroy, null);
                    await this.server.waitAsync(10);
                }
            }

            return ECode.Success;
        }
    }
}
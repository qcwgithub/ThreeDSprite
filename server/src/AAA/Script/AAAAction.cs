using System.Collections;
using System.Collections.Generic;

public class AAAAction : AAAHandler
{
    public override MsgType msgType { get { return MsgType.ServerAction; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse res)
    {
        var msg = _msg as MsgAAAAction;

        this.logger.info("%s", this.msgName);
        var aaaData = this.server.aaaData;

        if (msg.active != null)
        {
            aaaData.active = msg.active == "true";
        }

        if (msg.pmPlayerRunScript != null)
        {
            for (int i = 0; i < msg.pmPlayerRunScript.playerIds.Count; i++)
            {
                var playerId = msg.pmPlayerRunScript.playerIds[i];
                var player = aaaData.GetPlayerInfo(playerId);
                if (player == null)
                {
                    this.logger.info("%s playerRunScript player==null, playerId: %d", this.msgName, playerId);
                    continue;
                }
                var pm = aaaData.GetPlayerManagerInfo(player.pmId);
                if (pm == null)
                {
                    this.logger.info("%s playerRunScript pm==null, playerId: %d, pmId: %d", this.msgName, playerId, player.pmId);
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
                yield return this.server.baseScript.sendYield(pm.socket, MsgType.ServerAction, msgAction, res);
                yield return this.server.baseScript.waitYield(10);
            }
        }

        if (msg.destroyAll == "true")
        {
            while (true)
            {
                this.logger.info("%s destroyAllPlayers left %d", this.msgName, aaaData.playerInfos.Count);
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

                MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                this.server.baseScript.sendToSelf(MsgType.AAADestroyPlayer, msgDestroy);
                yield return this.server.baseScript.waitYield(10);
            }
        }

        if (msg.destroyPlayerIds != null)
        {
            for (int i = 0; i < msg.destroyPlayerIds.Count; i++)
            {
                this.logger.info("%s destroyPlayerIds left %d", this.msgName, msg.destroyPlayerIds.Count - i);
                var playerId = msg.destroyPlayerIds[i];
                if (!aaaData.playerInfos.ContainsKey(playerId))
                {
                    continue;
                }
                MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = playerId, place = this.msgName };
                this.server.baseScript.sendToSelf(MsgType.AAADestroyPlayer, msgDestroy);
                yield return this.server.baseScript.waitYield(10);
            }
        }

        res.err = ECode.Success;
        res.res = null;
    }
}
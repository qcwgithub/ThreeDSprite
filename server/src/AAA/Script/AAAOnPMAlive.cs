
using System.Collections;

public class AAAOnPMAlive : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAAOnPMAlive; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgPMAlive;
        var data = this.aaaData;
        var script = this.aaaScript;
        var logger = this.logger;

        this.baseScript.addKnownLoc(msg.loc);

        var newAdd = false;
        var pm = data.GetAAAPlayerManagerInfo(msg.id);
        if (pm == null)
        {
            logger.info("pm connected, id: " + msg.id);
            newAdd = true;

            pm = new AAAPlayerManagerInfo();
            pm.id = msg.id;
            data.playerManagerInfos.Add(msg.id, pm);
        }

        // 如果AAA挂，尝试恢复玩家数据
        if (msg.playerList != null)
        {
            if (msg.playerList.Count > 0)
            {
                logger.info("recover player ids...length: %d, pmId: %d", msg.playerList.Count, pm.id);
            }

            for (int i = 0; i < msg.playerList.Count; i++)
            {
                var playerId = msg.playerList[i];
                var player = data.GetPlayerInfo(playerId);
                if (player != null)
                {
                    if (player.pmId != pm.id)
                    {
                        this.server.baseScript.error("player pm conflict, player.pmId: %d, pm.id: %d", player.pmId, pm.id);
                    }
                }
                else
                {
                    logger.warn("recover playerId: %d, pmId: %d", playerId, pm.id);
                    player = new AAAPlayerInfo();
                    player.id = playerId;
                    player.pmId = pm.id;
                    // player.socket = null;
                    data.playerInfos.Add(playerId, player);
                }
            }
        }

        pm.socket = socket;
        pm.playerCount = msg.playerCount;
        pm.allowNewPlayer = msg.allowNewPlayer;

        // this.baseScript.removePending(this.server.networkHelper.getSocketId(socket));

        if (!data.pmReady && data.pmReadyTimer == -1)
        {
            // 延迟5秒再开始接受客户端连接
            data.pmReadyTimer = setTimeout(() =>
            {
                data.pmReady = true;
                data.pmReadyTimer = null;
            }, 5000);
        }

        r.err = ECode.Success;
        r.res = { requirePlayerList: newAdd };
    }
}
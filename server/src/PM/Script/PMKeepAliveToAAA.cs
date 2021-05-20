using System.Collections;
using System.Collections.Generic;

public class PMKeepAliveToAAA : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMKeepAliveToAAA; } }
    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        PMData pmData = this.pmData; var alive = this.pmData.alive;
        var s = pmData.aaaSocket;
        if (!this.server.netProto.isConnected(s))
        {
            alive.count = 10;
            r.err = ECode.Success;
            yield break;
        }

        if (alive.first || alive.requirePlayerList)
        {

        }
        else
        {
            // 保持连接的情况下，10秒一次
            alive.count++;
            if (alive.count < 10)
            {
                r.err = ECode.Success;
                yield break;
            }
        }

        alive.count = 0;
        alive.first = false;

        List<int> playerList = null;
        if (alive.requirePlayerList)
        {
            this.logger.info("alive.requirePlayerList = true");
            alive.requirePlayerList = false;
            playerList = new List<int>();
            foreach (var kv in pmData.playerInfos)
            {
                playerList.Add(kv.Key);
            }
        }

        var msgAlive = new MsgPMAlive()
        {
            id = this.baseData.id,
            playerCount = pmData.playerInfos.Count,
            loc = this.baseScript.myLoc(),
            playerList = playerList,
            allowNewPlayer = pmData.allowNewPlayer,
        };
        yield return this.baseScript.sendYield(s, MsgType.AAAOnPMAlive, msgAlive, r);

        if (r.err != ECode.Success)
        {
            this.baseScript.error("PMKeepAliveToAAA error: " + r.err);
        }
        else
        {
            this.pmData.aaaReady = true;
            if (r.res && r.res.requirePlayerList)
            {
                alive.requirePlayerList = true;
            }
        }

        r.err = ECode.Success;
        yield break;
    }
}
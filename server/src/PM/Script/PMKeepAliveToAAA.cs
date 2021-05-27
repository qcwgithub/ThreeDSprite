using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PMKeepAliveToAAA : PMHandler
{
    public override MsgType msgType { get { return MsgType.PMKeepAliveToAAA; } }
    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        PMData pmData = this.pmData; var alive = this.pmData.alive;
        var s = pmData.aaaSocket;
        if (!s.isConnected())
        {
            alive.count = 10;
            return ECode.Success;
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
                return ECode.Success;
            }
        }

        alive.count = 0;
        alive.first = false;

        List<int> playerList = null;
        if (alive.requirePlayerList)
        {
            this.logger.Info("alive.requirePlayerList = true");
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
        var r = await s.sendAsync(MsgType.AAAOnPMAlive, msgAlive);

        if (r.err != ECode.Success)
        {
            this.logger.Error("PMKeepAliveToAAA error: " + r.err);
        }
        else
        {
            this.pmData.aaaReady = true;
            if (r.res != null && (r.res as ResPMAlive).requirePlayerList)
            {
                alive.requirePlayerList = true;
            }
        }

        return ECode.Success;
    }
}
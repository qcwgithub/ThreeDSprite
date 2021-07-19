using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMKeepAliveToAAA : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMKeepAliveToAAA; } }
        public override async Task<MyResponse> handle(TcpClientData socket/* null */, object _msg /* null */)
        {
            PMData pmData = this.data; var alive = this.data.alive;
            if (!this.server.tcpClientScript.isServerConnected(ServerConst.AAA_ID))
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
                    // this.server.logger.Info("(skip) keep alive to aaa V" + this.server.scriptDllVersion);
                    return ECode.Success;
                }
            }
            // this.server.logger.Info("keep alive to aaa V" + this.server.scriptDllVersion);

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
                loc = this.server.myLoc(),
                playerList = playerList,
                allowNewPlayer = pmData.allowNewPlayer,
            };
            var r = await this.server.tcpClientScript.sendToServerAsync(ServerConst.AAA_ID, MsgType.AAAOnPMAlive, msgAlive);

            if (r.err != ECode.Success)
            {
                this.logger.Error("PMKeepAliveToAAA error: " + r.err);
            }
            else
            {
                this.data.aaaReady = true;
                var resPMAlive = this.server.CastObject<ResPMAlive>(r.res);
                if (resPMAlive.requirePlayerList)
                {
                    alive.requirePlayerList = true;
                }
            }

            return ECode.Success;
        }

        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            this.server.setTimer(1, this.msgType, null);
            return r;
        }
    }
}
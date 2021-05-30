
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAOnPMAlive : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAAOnPMAlive; } }

        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            var msg = this.baseScript.decodeMsg<MsgPMAlive>(_msg);
            var data = this.aaaData;
            var script = this.aaaScript;
            var logger = this.logger;

            this.baseScript.addKnownLoc(msg.loc);

            var newAdd = false;
            var pm = data.GetPlayerManagerInfo(msg.id);
            if (pm == null)
            {
                logger.Info("pm connected, id: " + msg.id);
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
                    logger.InfoFormat("recover player ids...length: {0}, pmId: {1}", msg.playerList.Count, pm.id);
                }

                for (int i = 0; i < msg.playerList.Count; i++)
                {
                    var playerId = msg.playerList[i];
                    var player = data.GetPlayerInfo(playerId);
                    if (player != null)
                    {
                        if (player.pmId != pm.id)
                        {
                            this.server.logger.ErrorFormat("player pm conflict, player.pmId: {0}, pm.id: {1}", player.pmId, pm.id);
                        }
                    }
                    else
                    {
                        logger.WarnFormat("recover playerId: {0}, pmId: {1}", playerId, pm.id);
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
                data.pmReadyTimer = this.server.timerScript.setTimer(() =>
                {
                    data.pmReady = true;
                    data.pmReadyTimer = -1;
                }, 5000);
            }

            return new MyResponse(ECode.Success, new ResPMAlive { requirePlayerList = newAdd });
        }
    }
}
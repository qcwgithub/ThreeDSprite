
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAOnPMAlive : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAAOnPMAlive; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            // this.server.logger.InfoFormat("{0} V{1}", this.msgName, this.server.scriptDllVersion);

            var msg = this.server.CastObject<MsgPMAlive>(_msg);
            var data = this.aaaData;
            var script = this.aaaScript;
            var logger = this.logger;

            this.server.addKnownLoc(msg.loc);

            this.server.data.otherServerSockets[msg.pmId] = socket;

            var newAdd = false;
            var pm = data.GetPlayerManager(msg.pmId);
            if (pm == null)
            {
                logger.Info("pm connected, pmId: " + msg.pmId);
                newAdd = true;

                pm = new AAAPlayerManager();
                pm.pmId = msg.pmId;
                data.playerManagerDict.Add(msg.pmId, pm);
            }

            // 如果AAA挂，尝试恢复玩家数据
            if (msg.playerList != null)
            {
                logger.InfoFormat("recover player ids from pm{0}, count {1}", pm.pmId, msg.playerList.Count);

                for (int i = 0; i < msg.playerList.Count; i++)
                {
                    var playerId = msg.playerList[i];
                    var player = data.GetPlayer(playerId);
                    if (player != null && player.pmId > 0 && player.pmId != pm.pmId)
                    {
                        this.server.logger.ErrorFormat("player pm conflict, player.pmId: {0}, pmId: {1}", player.pmId, pm.pmId);
                        // 这种情况不修正 player.pmId，错了就错了，问题也不大
                    }
                    else
                    {
                        logger.WarnFormat("recover playerId: {0}, pmId: {1}", playerId, pm.pmId);
                        if (player == null)
                        {
                            player = new AAAPlayer();
                            player.playerId = playerId;
                            // player.socket = null;
                            data.playerDict.Add(playerId, player);
                        }
                        player.pmId = pm.pmId;
                    }
                }
            }

            pm.playerCount = msg.playerCount;
            pm.allowNewPlayer = msg.allowNewPlayer;

            // this.baseScript.removePending(this.server.networkHelper.getSocketId(socket));

            if (!data.pmReady && data.pmReadyTimer == 0)
            {
                // 延迟5秒再开始接受客户端连接
                data.pmReadyTimer = this.server.setTimer(5, MsgType.AAASetPMReady, null);
            }

            return new MyResponse(ECode.Success, new ResPMAlive { requirePlayerList = newAdd }).toTask();
        }
    }
}
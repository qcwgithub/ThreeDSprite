using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    public class LobbyOnBMAlive : LobbyHandler
    {
        public override MsgType msgType { get { return MsgType.LobbyOnBMAlive; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            // this.server.logger.InfoFormat("{0} V{1}", this.msgName, this.server.scriptDllVersion);

            var msg = this.server.CastObject<MsgBMAlive>(_msg);

            this.server.addKnownLoc(msg.loc);

            this.server.data.otherServerSockets[msg.bmId] = socket;

            var newAdd = false;
            var bmInfo = this.server.lobbyData.GetBMInfo(msg.bmId);
            if (bmInfo == null)
            {
                logger.Info("bm connected, id: " + msg.bmId);
                newAdd = true;

                bmInfo = new LobbyBMInfo();
                bmInfo.bmId = msg.bmId;
                bmInfo.battleCount = msg.battleCount;
                bmInfo.allowNewBattle = msg.allowNewBattle;
                bmInfo.battles = new Dictionary<int, LobbyBattleInfo>();
                this.server.lobbyData.bmInfos.Add(msg.bmId, bmInfo);
            }

            // 如果 Lobby 挂，尝试恢复 battle 数据
            if (msg.battles != null)
            {
                logger.InfoFormat("recover battles from bm{0}, count {1}", bmInfo.bmId, msg.battles.Count);

                foreach (LobbyBattleInfo battleInfo in msg.battles)
                {
                    bmInfo.battles.Add(battleInfo.battleId, battleInfo);
                    // this.server.lobbyData.battleInfos.Add(battleInfo.battleId, battleInfo);

                    foreach (var playerId in battleInfo.playerIds)
                    {
                        this.server.lobbyData.playerInfos.Add(playerId, 
                            new LobbyPlayerInfo{ playerId = playerId, bmId = msg.bmId, battleId = battleInfo.battleId });
                    }
                }
            }

            bmInfo.battleCount = msg.battleCount;
            bmInfo.allowNewBattle = msg.allowNewBattle;

            // this.baseScript.removePending(this.server.networkHelper.getSocketId(socket));

            if (!this.server.lobbyData.bmReady && this.server.lobbyData.bmReadyTimer == 0)
            {
                // 延迟5秒再开始接受客户端连接
                this.server.lobbyData.bmReadyTimer = this.server.setTimer(5, MsgType.LobbySetBMReady, null);
            }

            return new MyResponse(ECode.Success, new ResBMAlive { requireBattleList = newAdd }).toTask();
        }
    }
}
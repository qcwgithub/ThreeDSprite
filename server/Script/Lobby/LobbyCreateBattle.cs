using System.Threading.Tasks;
using Data;
using System.Collections.Generic;

namespace Script
{
    public class LobbyCreateBattle : LobbyHandler
    {
        public override MsgType msgType => MsgType.LobbyCreateBattle;

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            LobbyBMInfo bm = null;
            foreach (var kv in this.server.lobbyData.bmInfos)
            {
                var bmInfo = kv.Value;
                if (!bmInfo.allowNewBattle)
                {
                    continue;
                }

                if (!this.server.tcpClientScript.isServerConnected(bmInfo.bmId))
                {
                    continue;
                }

                if (bm == null || bmInfo.battleCount < bm.battleCount)
                {
                    bm = bmInfo;
                }
            }

            if (bm == null)
            {
                this.server.logger.Error("no available bm!");
                return ECode.NoAvailableBM.toTask();
            }

            LobbyBattleInfo battleInfo = new LobbyBattleInfo();
            battleInfo.bmId = bm.bmId;
            battleInfo.battleId = this.server.lobbyData.battleId++;
            battleInfo.playerIds = new List<int>();
            bm.battles.Add(battleInfo.battleId, battleInfo);

            ResLobbyCreateBattle res = new ResLobbyCreateBattle();
            res.bmId = bm.bmId;
            res.battleId = battleInfo.battleId;

            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
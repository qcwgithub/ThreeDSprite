using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    public class LobbyDestroyBattle : LobbyHandler
    {
        public override MsgType msgType { get { return MsgType.LobbyDestroyBattle; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLobbyDestroyBattle>(_msg);
            var bmInfo = this.server.lobbyData.GetBMInfo(msg.bmId);
            if (bmInfo == null)
            {
                this.server.logger.ErrorFormat("{0} bmId({1}) bmInfo==null", this.msgName, msg.bmId);
                return ECode.BMNotExist.toTask();
            }

            LobbyBattleInfo battleInfo;
            if (!bmInfo.battles.TryGetValue(msg.battleId, out battleInfo))
            {
                this.server.logger.ErrorFormat("{0} bmId({1}) battleId({2}) battleInfo==null", this.msgName, msg.bmId, msg.battleId);
                return ECode.BattleNotExist.toTask();
            }

            foreach (var playerId in battleInfo.playerIds)
            {
                this.server.lobbyData.playerDict.Remove(playerId);
            }

            bmInfo.battles.Remove(msg.battleId);

            return ECode.Success.toTask();
        }
    }
}
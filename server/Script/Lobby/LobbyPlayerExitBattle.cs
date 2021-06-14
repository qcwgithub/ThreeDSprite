using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LobbyPlayerExitBattle : LobbyHandler
    {
        public override MsgType msgType => MsgType.LobbyPlayerExitBattle;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgLobbyPlayerExitBattle>(_msg);
            LobbyPlayerInfo lobbyPlayerInfo;
            if (!this.server.lobbyData.playerInfos.TryGetValue(msg.playerId, out lobbyPlayerInfo))
            {
                return ECode.PlayerNotInBattle;
            }

            LobbyBMInfo bmInfo = this.server.lobbyData.bmInfos[lobbyPlayerInfo.bmId];

            var msg2 = new MsgBMPlayerExit();
            msg2.playerId = msg.playerId;
            var r = await this.server.tcpClientScript.sendToServerAsync(bmInfo.bmId, MsgType.BMPlayerExit, msg);
            if (r.err != ECode.Success)
            {
                return r.err;
            }

            LobbyBattleInfo battleInfo = bmInfo.battles[lobbyPlayerInfo.battleId];
            battleInfo.playerIds.Remove(msg.playerId);

            this.server.lobbyData.playerInfos.Remove(msg.playerId);
            return ECode.Success;
        }
    }
}
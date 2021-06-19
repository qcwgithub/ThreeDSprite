using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerExit : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerExit;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMPlayerExit>(_msg);
            
            BMBattleInfo battleInfo;
            if (!this.server.bmData.battleInfos.TryGetValue(msg.battleId, out battleInfo))
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayerInfo playerInfo;
            if (!battleInfo.players.TryGetValue(msg.playerId, out playerInfo))
            {
                return ECode.PlayerNotInBattle.toTask();
            }

            battleInfo.players.Remove(msg.playerId);

            return ECode.Error.toTask();
        }
    }
}
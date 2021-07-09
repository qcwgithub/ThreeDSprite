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
            
            BMBattleInfo battle = this.server.bmData.GetBattle(msg.battleId);
            if (battle == null)
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayerInfo player = battle.GetPlayer(msg.playerId);
            if (player == null)
            {
                return ECode.PlayerNotInBattle.toTask();
            }

            battle.playerDict.Remove(msg.playerId);

            return ECode.Error.toTask();
        }
    }
}
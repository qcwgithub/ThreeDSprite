using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerExit : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerExit;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgBMPlayerExit>(_msg);
            
            BMBattle battle = this.server.bmData.GetBattle(msg.battleId);
            if (battle == null)
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayer player = battle.GetPlayer(msg.playerId);
            if (player == null)
            {
                return ECode.PlayerNotInBattle.toTask();
            }

            battle.playerDict.Remove(msg.playerId);

            return ECode.Error.toTask();
        }
    }
}
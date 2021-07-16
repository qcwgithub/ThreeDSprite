using System.Threading.Tasks;
using Data;

namespace Script
{
    // from lobby
    public class BMPlayerEnter : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerEnter;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMPlayerEnter>(_msg);
            this.server.logger.Info(this.msgName + ", playerId: " + msg.playerId);

            BMBattleInfo battle = this.server.bmData.GetBattle(msg.battleId);
            if (battle == null)
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayerInfo player = battle.GetPlayer(msg.playerId);
            if (player != null)
            {
                return ECode.BattleAlreadyContainsPlayer.toTask();
            }

            player = this.server.mainScript.addPlayer(battle, msg.playerId);            
            player.token = "";
            player.socket = null;

            var res = new ResBMPlayerEnter();
            res.token = "";
            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
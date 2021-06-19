using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerLogin : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerLogin;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMPlayerLogin>(_msg);
            
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

            if (msg.token != playerInfo.token)
            {
                return ECode.InvalidToken.toTask();
            }
            
            return ECode.Success.toTask();
        }
    }
}
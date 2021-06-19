using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMPlayerEnter : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerEnter;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMPlayerEnter>(_msg);
            BMBattleInfo battleInfo;
            if (!this.server.bmData.battleInfos.TryGetValue(msg.battleId, out battleInfo))
            {
                return ECode.BattleNotExist.toTask();
            }

            if (battleInfo.players.ContainsKey(msg.playerId))
            {
                return ECode.BattleAlreadyContainsPlayer.toTask();
            }

            BMPlayerInfo playerInfo = new BMPlayerInfo();
            playerInfo.playerId = msg.playerId;
            battleInfo.players.Add(msg.playerId, playerInfo);

            var res = new ResBMPlayerEnter();
            res.token = "";
            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
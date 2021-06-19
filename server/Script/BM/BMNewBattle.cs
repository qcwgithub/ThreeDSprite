using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMNewBattle : BMHandler
    {
        public override MsgType msgType => MsgType.BMNewBattle;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMCreateBattle>(_msg);
            if (this.server.bmData.battleInfos.ContainsKey(msg.battleId))
            {
                return ECode.Error.toTask();
            }

            BMBattleInfo battleInfo = new BMBattleInfo();
            battleInfo.battleId = msg.battleId;
            this.server.bmData.battleInfos.Add(msg.battleId, battleInfo);
            
            return ECode.Success.toTask();
        }
    }
}
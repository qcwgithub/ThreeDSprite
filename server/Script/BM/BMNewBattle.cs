using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMNewBattle : BMHandler
    {
        public override MsgType msgType => MsgType.BMNewBattle;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgBMCreateBattle>(_msg);
            if (null != this.server.bmData.GetBattle(msg.battleId))
            {
                return ECode.BattleIdAlreadyExist.toTask();
            }

            if (null == this.server.bmData.GetTilemapData(msg.mapId))
            {
                return ECode.MapDataNotExist.toTask();
            }

            BMBattle battle = this.server.createScript.newBattle(msg.battleId, msg.mapId);
            this.server.bmData.battleDict.Add(msg.battleId, battle);

            return ECode.Success.toTask();
        }
    }
}
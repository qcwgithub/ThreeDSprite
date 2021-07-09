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
            if (this.server.bmData.battleDict.ContainsKey(msg.battleId))
            {
                return ECode.MapAlreadyExist.toTask();
            }

            btTilemapData tilemapData;
            if (!this.server.bmData.tilemapDatas.TryGetValue(msg.mapId, out tilemapData))
            {
                return ECode.MapDataNotExist.toTask();
            }

            BMBattleInfo battle = this.server.createScript.newBattle(msg.battleId, msg.mapId);
            this.server.bmData.battleDict.Add(msg.battleId, battle);

            return ECode.Success.toTask();
        }
    }
}
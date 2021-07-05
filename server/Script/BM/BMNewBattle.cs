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
                return ECode.MapAlreadyExist.toTask();
            }

            btTilemapData tilemapData;
            if (!this.server.bmData.tilemapDatas.TryGetValue(msg.mapId, out tilemapData))
            {
                return ECode.MapDataNotExist.toTask();
            }

            BMBattleInfo battleInfo = new BMBattleInfo();
            battleInfo.battleId = msg.battleId;
            battleInfo.battle = this.server.mainScript.newBattle(msg.mapId);

            this.server.bmData.battleInfos.Add(msg.battleId, battleInfo);

            return ECode.Success.toTask();
        }
    }
}
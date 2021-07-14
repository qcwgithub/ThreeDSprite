using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMMove : BMHandler
    {
        public override MsgType msgType => MsgType.BMMove;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<BMMsgMove>(_msg);
            BMPlayerInfo player = this.getPlayer(socket);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} player == null!!", this.msgName);
                return ECode.PlayerNotExist.toTask();
            }

            if (player.character == null)
            {
                return ECode.PlayerHasNoCharacter.toTask();
            }

            // player.battleInfo

            // BMBattleInfo battleInfo = this.server.bmData.GetBattleInfo(player.battleId);
            // if (battleInfo == null)
            // {
            //     return ECode.BattleNotExist.toTask();
            // }

            // this.logger.InfoFormat("move {0} {1},{2},{3}", msg.id, msg.moveDir.x, msg.moveDir.y, msg.moveDir.z);
            

            ECode e = this.server.moveScript.characterMove(player.battle, player.character.id, msg.moveDir);
            if (e != ECode.Success)
            {
                return e.toTask();
            }

            var res = new BMResMove();
            res.characterId = player.character.id;
            res.moveDir = msg.moveDir;
            this.broadcast(player.battle, this.msgType, res);
            
            return ECode.Success.toTask();
        }
    }
}
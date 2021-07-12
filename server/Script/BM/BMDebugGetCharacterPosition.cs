using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMDebugGetCharacterPosition : BMHandler
    {
        public override MsgType msgType => MsgType.BMDebugGetCharacterPosition;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<BMMsgDebugGetCharacterPosition>(_msg);
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

            btCharacter character = player.battle.GetCharacter(msg.characterId);
            if (character == null)
            {
                return ECode.CharacterNotExist.toTask();
            }

            return new MyResponse(ECode.Success, new BMResDebugGetCharacterPosition { position = character.pos }).toTask();
        }
    }
}
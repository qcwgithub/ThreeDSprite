using System.Threading.Tasks;
using Data;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class BMPlayerLogin : BMHandler
    {
        public override MsgType msgType => MsgType.BMPlayerLogin;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<BMMsgPlayerLogin>(_msg);

            BMBattleInfo battle = this.server.bmData.GetBattle(msg.battleId);
            if (battle == null)
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayerInfo player = battle.GetPlayer(msg.playerId);
            if (player == null)
            {
                return ECode.PlayerNotInBattle.toTask();
            }

            // if (msg.token != playerInfo.token)
            // {
            //     return ECode.InvalidToken.toTask();
            // }

            if (player.battle != null && player.battle != battle)
            {
                return ECode.Error.toTask();
            }

            player.socket = socket;
            player.battle = battle;

            ////////////////////////////////////////////////////////////////////////

            if (player.character == null)
            {
                // playerInfo.characterId = this
                player.character = this.server.mainScript.addCharacter(battle, battle.nextCharacterId++, player.playerId);

                // random walkable
                btIWalkable chWalkable;
                Vector3 chPos;
                this.server.moveScript.randomWalkable(battle, out chWalkable, out chPos);
                player.character.walkable = chWalkable;
                player.character.pos = chPos;
            }

            ////////////////////////////////////////////////////////////////////////
            // send whole game

            var res = new BMResPlayerLogin();
            res.battle = battle;
            res.characterDict = new Dictionary<int, MCharacter>();
            foreach (var kv in battle.playerDict)
            {
                var player2 = kv.Value;
                if (player2.character != null)
                {
                    var mc = new MCharacter();
                    mc.characterId = player2.character.id;
                    mc.pos = FVector3.FromVector3(player2.character.pos);
                    mc.moveDir = FVector3.FromVector3(player2.character.moveDir);
                    mc.walkableId = (player2.character.walkable as btObject).id;
                    res.characterDict.Add(kv.Key, mc);
                }
            }

            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
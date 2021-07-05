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

            if (playerInfo.battleId > 0 && playerInfo.battleId != msg.battleId)
            {
                return ECode.Error.toTask();
            }

            // add into battle
            playerInfo.battleId = msg.battleId;
            if (playerInfo.character == null)
            {
                // playerInfo.characterId = this
                playerInfo.character = this.server.mainScript.addCharacter(battleInfo.battle);

                // random walkable
                btIWalkable chWalkable;
                Vector3 chPos;
                this.server.moveScript.randomWalkable(battleInfo.battle, out chWalkable, out chPos);
                playerInfo.character.walkable = chWalkable;
                playerInfo.character.pos = chPos;
            }

            var res = new ResBMPlayerLogin();
            res.battleId = battleInfo.battleId;
            res.mapId = battleInfo.battle.mapId;
            res.characterId = playerInfo.character.id;

            //------------------------------
            res.battleData = new MBattleData();
            res.battleData.characters = new List<MCharacter>();
            foreach (var kv in battleInfo.battle.characters)
            {
                var mc = new MCharacter();
                mc.id = kv.Value.id;
                mc.pos = FVector3.FromVector3(kv.Value.pos);
                mc.moveDir = FVector3.FromVector3(kv.Value.moveDir);
                res.battleData.characters.Add(mc);
            }

            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}
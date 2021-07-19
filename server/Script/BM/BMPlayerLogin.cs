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
            var msg = this.server.CastObject<BMMsgPlayerLogin>(_msg);

            BMBattle battle = this.server.bmData.GetBattle(msg.battleId);
            if (battle == null)
            {
                return ECode.BattleNotExist.toTask();
            }

            BMPlayer player = battle.GetPlayer(msg.playerId);
            if (player == null)
            {
                return ECode.PlayerNotInBattle.toTask();
            }

            // if (msg.token != playerInfo.token)
            // {
            //     return ECode.InvalidToken.toTask();
            // }

            this.server.tcpClientScript.bindPlayer(socket, player, 0);

            if (player.battle != null && player.battle != battle)
            {
                return ECode.Error.toTask();
            }

            player.socket = socket;
            player.battle = battle;

            ////////////////////////////////////////////////////////////////////////

            if (player.character == null)
            {
                // random walkable
                btIWalkable chWalkable;
                Vector3 chPos;
                this.server.moveScript.randomWalkable(battle, out chWalkable, out chPos);

                // playerInfo.characterId = this
                player.character = this.server.mainScript.addCharacter(battle, battle.nextCharacterId++, player.playerId, ((btObject)chWalkable).id, chPos, Vector3.zero);

                this.broadcastAddCharacter(battle, player);
            }

            ////////////////////////////////////////////////////////////////////////
            // send whole game

            var msgBattle = new BMMsgBattle();
            msgBattle.battle = battle;
            player.socket.send(MsgType.BMBattle, msgBattle, null);
            return ECode.Success.toTask();
        }

        void broadcastAddCharacter(BMBattle battle, BMPlayer player)
        {
            var msg = new BMMsgAddCharacter();
            msg.character = player.character;
            this.broadcast(battle, MsgType.BMAddCharacter, msg, player.playerId);
        }
    }
}
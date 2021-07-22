using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMainScript : btScriptBase
    {
        public BMPlayer addPlayer(BMBattle battleInfo, int playerId, int battleId, int characterConfigId)
        {
            var player = new BMPlayer();
            // [0]
            player.playerId = playerId;
            // [1]
            player.battleId = battleId;
            // [2]
            player.characterConfigId = characterConfigId;
            // player.token = "";
            // init
            // player.socket = null;
            player.battle = null;
            player.character = null;

            battleInfo.playerDict.Add(playerId, player);
            return player;
        }

        public btCharacter addCharacter(BMBattle battle, int characterId, int playerId, int characterConfigId, int walkableId, Vector3 pos, Vector3 moveDir)
        {
            btCharacter character = new btCharacter();
            // [0]
            character.type = btObjectType.character;
            // [1]
            character.id = characterId;
            // [2]
            character.playerId = playerId;
            // [3]
            character.characterConfigId = characterConfigId;
            // [4]
            character.walkableId = walkableId;
            // [5]
            character.pos = pos;
            // [6]
            character.moveDir = moveDir;
            
            character.bodyType = q3BodyType.eDynamicBody;
            character.worldMin = new Vector3(-0.5f, 0f, 0f);
            character.worldMax = new Vector3(0.5f, 2f, 0f);
            this.addObject(battle, character);
            this.scripts.moveScript.setObjectPosition(battle, character, pos);

            if (character.walkableId > 0)
            {
                character.walkable = battle.walkables.Find(_ => ((btObject)_).id == character.walkableId);
            }

            return character;
        }

        void addToPhysicsScene(BMBattle battle, btObject obj)
        {
            Vector3 center = (obj.worldMin + obj.worldMax) / 2;

            // body 的位置在正中间（这个不是必须的）
            obj.body = Qu3eApi.SceneAddBody(battle.physicsScene, obj.bodyType, center.x, center.y, center.z);
            battle.body2Objects.Add(obj.body, obj);
            
            // 添加一个 box
            Vector3 size = obj.worldMax - obj.worldMin;
            Vector3 extends = size / 2;
            Qu3eApi.BodyAddBox(obj.body, 0f, 0f, 0f, extends.x, extends.y, extends.z);
        }

        public void addObject(BMBattle battle, btObject obj)
        {
            battle.objects.Add(obj.id, obj);
            if (obj is btCharacter character)
            {
                battle.characters.Add(obj.id, character);
            }
            if (obj is btIWalkable walkable)
            {
                battle.walkables.Add(walkable);
            }
            // if (obj is btIObstacle obstacle)
            // {
            //     battle.obstacles.Add(obstacle);
            // }
            // if (obj is btTree tree)
            // {
            //     battle.trees.Add(tree);
            // }
            this.addToPhysicsScene(battle, obj);
        }

    }
}
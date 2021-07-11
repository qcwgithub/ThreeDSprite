using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMainScript : btScriptBase
    {
        public BMPlayerInfo addPlayer(BMBattleInfo battleInfo,  int playerId)
        {
            var playerInfo = new BMPlayerInfo();
            playerInfo.playerId = playerId;
            // playerInfo.token = "";
            // init
            // playerInfo.socket = null;
            playerInfo.battle = null;
            playerInfo.character = null;

            battleInfo.playerDict.Add(playerId, playerInfo);
            return playerInfo;
        }

        void addToPhysicsScene(BMBattleInfo battle, btObject obj)
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

        public void addObject(BMBattleInfo battle, btObject obj)
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

        public btCharacter addCharacter(BMBattleInfo battle, int characterId, int playerId, Vector3 pos)
        {
            btCharacter character = new btCharacter();
            character.playerId = playerId;
            character.type = btObjectType.character;
            character.id = characterId;
            character.bodyType = q3BodyType.eDynamicBody;
            character.pos = Vector3.zero;
            character.worldMin = new Vector3(-0.5f, 0f, 0f);
            character.worldMax = new Vector3(0.5f, 2f, 0f);
            this.addObject(battle, character);
            this.scripts.moveScript.setObjectPosition(battle, character, pos);
            return character;
        }

    }
}
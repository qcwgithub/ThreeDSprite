using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMainScript : btScriptBase
    {
        public btBattle createBattle(btTilemapData tilemapData, Dictionary<string, btTilesetConfig> tilesetConfigs)
        {
            btBattle battle = new btBattle();
            this.initBattle(battle, tilemapData, tilesetConfigs);
            return battle;
        }

        public void destroyBattle(btBattle battle)
        {
            Qu3eApi.SceneDestroy(battle.physicsScene);
            battle.physicsScene = IntPtr.Zero;
        }

        public void initBattle(btBattle battle, btTilemapData tilemapData, Dictionary<string, btTilesetConfig> tilesetConfigs)
        {
            battle.tilemapData = tilemapData;
            battle.tilesetConfigs = tilesetConfigs;
            battle.physicsScene = Qu3eApi.CreateScene();
            battle.onBeginContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onBeginContact(battle, bodyA, boxA, bodyB, boxB));

            battle.onEndContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onEndContact(battle, bodyA, boxA, bodyB, boxB));

            Qu3eApi.SceneSetContactListener(battle.physicsScene, battle.onBeginContactDel, battle.onEndContactDel);

            Vector3 mapOffset = Vector3.zero; // to do ?

            foreach (btTileLayerData layerData in tilemapData.layerDatas)
            {
                Vector3 layerOffset = FVector3.ToVector3(layerData.offset);

                if (layerData.objectType == btObjectType.floor)
                {
                    Vector3 min = Vector3.zero;
                    Vector3 max = Vector3.zero;
                    bool first = true;
                    foreach (btTileData tileData in layerData.tileDatas)
                    {
                        btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                        if (tileConfig.shape == btShape.xz)
                        {
                            Vector3 mi = FVector3.ToVector3(tileData.position);
                            Vector3 ma = new Vector3(
                                mi.x + tileConfig.size.x,
                                mi.y + tileConfig.size.y,
                                mi.z + tileConfig.size.z);

                            if (first || mi.x < min.x) min.x = mi.x;
                            if (first || mi.y < min.y) min.y = mi.y;
                            if (first || mi.z < min.z) min.z = mi.z;

                            if (first || ma.x > max.x) max.x = ma.x;
                            if (first || ma.y > max.y) max.y = ma.y;
                            if (first || ma.z > max.z) max.z = ma.z;

                            first = false;
                        }
                    }

                    Vector3 worldMin = min + mapOffset + layerOffset;
                    Vector3 worldMax = max + mapOffset + layerOffset;

                    btFloor floor = new btFloor(battle, layerData.id, worldMin, worldMax);
                    battle.walkables.Add(floor);
                    battle.DictObjects.Add(floor.id, floor);
                }
                else if (layerData.objectType == btObjectType.stair)
                {
                    Vector3 min = Vector3.zero;
                    Vector3 max = Vector3.zero;
                    bool first = true;
                    foreach (btTileData tileData in layerData.tileDatas)
                    {
                        btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                        if (tileConfig.shape == btShape.cube)
                        {
                            Vector3 mi = FVector3.ToVector3(tileData.position);
                            Vector3 ma = new Vector3(
                                mi.x + tileConfig.size.x,
                                mi.y + tileConfig.size.y,
                                mi.z + tileConfig.size.z);

                            if (first || mi.x < min.x) min.x = mi.x;
                            if (first || mi.y < min.y) min.y = mi.y;
                            if (first || mi.z < min.z) min.z = mi.z;

                            if (first || ma.x > max.x) max.x = ma.x;
                            if (first || ma.y > max.y) max.y = ma.y;
                            if (first || ma.z > max.z) max.z = ma.z;

                            first = false;
                        }
                    }

                    Vector3 worldMin = min + mapOffset + layerOffset;
                    Vector3 worldMax = max + mapOffset + layerOffset;

                    btStair stair = new btStair(battle, layerData.id, layerData.stairDir, worldMin, worldMax);
                    battle.walkables.Add(stair);
                    battle.DictObjects.Add(stair.id, stair);
                }
                else if (layerData.objectType == btObjectType.wall)
                {
                    Vector3 min = Vector3.zero;
                    Vector3 max = Vector3.zero;
                    bool first = true;
                    foreach (btTileData tileData in layerData.tileDatas)
                    {
                        btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                        if (tileConfig.shape == btShape.xy)
                        {
                            Vector3 mi = FVector3.ToVector3(tileData.position);
                            Vector3 ma = new Vector3(
                                mi.x + tileConfig.size.x,
                                mi.y + tileConfig.size.y,
                                mi.z + tileConfig.size.z);

                            if (first || mi.x < min.x) min.x = mi.x;
                            if (first || mi.y < min.y) min.y = mi.y;
                            if (first || mi.z < min.z) min.z = mi.z;

                            if (first || ma.x > max.x) max.x = ma.x;
                            if (first || ma.y > max.y) max.y = ma.y;
                            if (first || ma.z > max.z) max.z = ma.z;

                            first = false;
                        }
                    }

                    Vector3 worldMin = min + mapOffset + layerOffset;
                    Vector3 worldMax = max + mapOffset + layerOffset;

                    btWall wall = new btWall(battle, layerData.id, worldMin, worldMax);
                    battle.obstacles.Add(wall);
                    battle.DictObjects.Add(wall.id, wall);
                }

                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    switch (tileConfig.objectType)
                    {
                        case btObjectType.box_obstacle:
                            {
                                btBoxObstacle obstacle = new btBoxObstacle(battle, mapOffset + layerOffset, tileData, tileConfig);
                                battle.obstacles.Add(obstacle);
                                battle.DictObjects.Add(obstacle.id, obstacle);
                            }
                            break;

                        case btObjectType.tree:
                            {
                                btTree tree = new btTree(battle, mapOffset + layerOffset, tileData, tileConfig);
                                battle.trees.Add(tree);
                                battle.DictObjects.Add(tree.id, tree);
                            }
                            break;
                    }
                }
            }

            foreach (var kv in battle.DictObjects)
            {
                kv.Value.AddToPhysicsScene();
            }
        }

        public btCharacter addCharacter(btBattle battle)
        {
            btCharacter lChar = new btCharacter(battle, 10000);
            battle.DictObjects.Add(lChar.id, lChar);
            lChar.AddToPhysicsScene();
            return lChar;
        }

        void onBeginContact(btBattle battle, IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
        {
            btObject objectA;
            if (!battle.body2Objects.TryGetValue(bodyA, out objectA))
            {
                return;
            }

            btObject objectB;
            if (!battle.body2Objects.TryGetValue(bodyB, out objectB))
            {
                return;
            }
            Debug.Log(string.Format("OnBeginContact {0} - {1}", objectA, objectB));
            objectA.Collidings.Add(new btObject_Time { obj = objectB });
            objectB.Collidings.Add(new btObject_Time { obj = objectA });
        }

        void onEndContact(btBattle battle, IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
        {
            // Debug.LogWarning(string.Format("OnEndContact"));

            btObject objectA;
            if (!battle.body2Objects.TryGetValue(bodyA, out objectA))
            {
                return;
            }

            btObject objectB;
            if (!battle.body2Objects.TryGetValue(bodyB, out objectB))
            {
                return;
            }

            Debug.Log(string.Format("OnEndContact {0} x {1}", objectA, objectB));
            for (int i = 0; i < objectA.Collidings.Count; i++)
            {
                if (objectA.Collidings[i].obj == objectB)
                {
                    objectA.Collidings.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < objectB.Collidings.Count; i++)
            {
                if (objectB.Collidings[i].obj == objectA)
                {
                    objectB.Collidings.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
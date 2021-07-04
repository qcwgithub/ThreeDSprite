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

                    btFloor floor = new btFloor();
                    floor.battle = battle;
                    floor.type = btObjectType.floor;
                    floor.id = layerData.id;
                    floor.worldMin = worldMin;
                    floor.worldMax = worldMax;
                    floor.y = worldMin.y;

                    battle.walkables.Add(floor);
                    battle.objects.Add(floor.id, floor);
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

                    btStair stair = new btStair();
                    stair.battle = battle;
                    stair.type = btObjectType.stair;
                    stair.id = layerData.id;
                    stair.worldMin = worldMin;
                    stair.worldMax = worldMax;
                    stair.dir = layerData.stairDir;
                    battle.walkables.Add(stair);
                    battle.objects.Add(stair.id, stair);
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

                    btWall wall = new btWall();
                    wall.battle = battle;
                    wall.type = btObjectType.wall;
                    wall.id = layerData.id;
                    wall.worldMin = worldMin;
                    wall.worldMax = worldMax;
                    battle.objects.Add(wall.id, wall);
                }

                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    switch (tileConfig.objectType)
                    {
                        case btObjectType.box_obstacle:
                            {
                                btBoxObstacle obstacle = new btBoxObstacle();
                                obstacle.battle = battle;
                                obstacle.type = btObjectType.box_obstacle;
                                obstacle.id = layerData.id;
                                obstacle.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                obstacle.worldMax = obstacle.worldMin + FVector3.ToVector3(tileConfig.size);
                                obstacle.tileConfig = tileConfig;
                                obstacle.data = tileData;
                                battle.obstacles.Add(obstacle);
                                battle.objects.Add(obstacle.id, obstacle);
                            }
                            break;

                        case btObjectType.tree:
                            {
                                btTree tree = new btTree();
                                tree.battle = battle;
                                tree.type = btObjectType.tree;
                                tree.id = layerData.id;
                                tree.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                tree.worldMax = tree.worldMin + FVector3.ToVector3(tileConfig.size);
                                tree.tileConfig = tileConfig;
                                tree.data = tileData;
                                battle.trees.Add(tree);
                                battle.objects.Add(tree.id, tree);
                            }
                            break;
                    }
                }
            }

            foreach (var kv in battle.objects)
            {
                kv.Value.AddToPhysicsScene();
            }
        }

        public btCharacter addCharacter(btBattle battle)
        {
            btCharacter character = new btCharacter();
            character.battle = battle;
            character.type = btObjectType.character;
            character.id = 10000;
            battle.objects.Add(character.id, character);
            battle.characters.Add(character.id, character);
            character.AddToPhysicsScene();
            return character;
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
            objectA.collidings.Add(new btObject_Time { obj = objectB });
            objectB.collidings.Add(new btObject_Time { obj = objectA });
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
            for (int i = 0; i < objectA.collidings.Count; i++)
            {
                if (objectA.collidings[i].obj == objectB)
                {
                    objectA.collidings.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < objectB.collidings.Count; i++)
            {
                if (objectB.collidings[i].obj == objectA)
                {
                    objectB.collidings.RemoveAt(i);
                    i--;
                }
            }
        }

        public void update(btBattle battle, float dt)
        {
            battle.updating = true;
            Qu3eApi.SceneStep(battle.physicsScene);
            battle.updating = false;
        }
    }
}
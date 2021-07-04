using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMainScript : btScriptBase
    {
        public void destroyBattle(btBattle battle)
        {
            Qu3eApi.SceneDestroy(battle.physicsScene);
            battle.physicsScene = IntPtr.Zero;
        }

        void calcWorldBounds(Vector3 parentOffset, btTileLayerData layerData, btShape includeShape, out Vector3 worldMin, out Vector3 worldMax)
        {
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;
            bool first = true;
            foreach (btTileData tileData in layerData.tileDatas)
            {
                btTileConfig tileConfig = battle.tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                if (tileConfig.shape == includeShape)
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

            worldMin = min + parentOffset;
            worldMax = max + parentOffset;
        }

        public void initBattle(btTilemapData tilemapData, Dictionary<string, btTilesetConfig> tilesetConfigs)
        {
            battle.tilemapData = tilemapData;
            battle.tilesetConfigs = tilesetConfigs;
            battle.physicsScene = Qu3eApi.CreateScene();
            battle.onBeginContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onBeginContact(bodyA, boxA, bodyB, boxB));

            battle.onEndContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onEndContact(bodyA, boxA, bodyB, boxB));

            Qu3eApi.SceneSetContactListener(battle.physicsScene, battle.onBeginContactDel, battle.onEndContactDel);

            Vector3 mapOffset = Vector3.zero; // to do ?

            foreach (btTileLayerData layerData in tilemapData.layerDatas)
            {
                Vector3 layerOffset = FVector3.ToVector3(layerData.offset);

                if (layerData.objectType == btObjectType.floor)
                {
                    btFloor floor = new btFloor();
                    floor.type = btObjectType.floor;
                    floor.id = layerData.id;
                    this.calcWorldBounds(mapOffset + layerOffset, layerData, btShape.xz, out floor.worldMin, out floor.worldMax);
                    this.addObject(floor);
                }
                else if (layerData.objectType == btObjectType.stair)
                {
                    btStair stair = new btStair();
                    stair.type = btObjectType.stair;
                    stair.id = layerData.id;
                    stair.dir = layerData.stairDir;
                    this.calcWorldBounds(mapOffset + layerOffset, layerData, btShape.cube, out stair.worldMin, out stair.worldMax);
                    this.addObject(stair);
                }
                else if (layerData.objectType == btObjectType.wall)
                {
                    btWall wall = new btWall();
                    wall.type = btObjectType.wall;
                    wall.id = layerData.id;
                    this.calcWorldBounds(mapOffset + layerOffset, layerData, btShape.xy, out wall.worldMin, out wall.worldMax);
                    this.addObject(wall);
                }

                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    switch (tileConfig.objectType)
                    {
                        case btObjectType.box_obstacle:
                            {
                                btBoxObstacle obstacle = new btBoxObstacle();
                                obstacle.type = btObjectType.box_obstacle;
                                obstacle.id = layerData.id;
                                obstacle.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                obstacle.worldMax = obstacle.worldMin + FVector3.ToVector3(tileConfig.size);
                                obstacle.tileConfig = tileConfig;
                                obstacle.data = tileData;
                                this.addObject(obstacle);
                            }
                            break;

                        case btObjectType.tree:
                            {
                                btTree tree = new btTree();
                                tree.type = btObjectType.tree;
                                tree.id = layerData.id;
                                tree.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                tree.worldMax = tree.worldMin + FVector3.ToVector3(tileConfig.size);
                                tree.tileConfig = tileConfig;
                                tree.data = tileData;
                                this.addObject(tree);
                            }
                            break;
                    }
                }
            }
        }

        void addToPhysicsScene(btObject obj)
        {
            Vector3 center = (obj.worldMin + obj.worldMax) / 2;
            Vector3 size = obj.worldMax - obj.worldMin;

            var body = Qu3eApi.SceneAddBody(battle.physicsScene, obj.bodyType, center.x, center.y, center.z);
            battle.body2Objects.Add(body, obj);
            
            Vector3 extends = size / 2;
            Qu3eApi.BodyAddBox(body, 0f, 0f, 0f, extends.x, extends.y, extends.z);
        }

        public void addObject(btObject obj)
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
            if (obj is btIObstacle obstacle)
            {
                battle.obstacles.Add(obstacle);
            }
            if (obj is btTree tree)
            {
                battle.trees.Add(tree);
            }
            this.addToPhysicsScene(obj);
        }

        public btCharacter addCharacter()
        {
            btCharacter character = new btCharacter();
            character.type = btObjectType.character;
            character.id = 10000;
            this.addObject(character);
            return character;
        }

        void onBeginContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
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

        void onEndContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
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

        public void update(float dt)
        {
            battle.updating = true;
            Qu3eApi.SceneStep(battle.physicsScene);
            battle.updating = false;
        }
    }
}
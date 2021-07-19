using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btCreateScript : btScriptBase
    {
        void calcWorldBounds(BMBattle battle, Vector3 parentOffset, btTileLayerData layerData, btShape includeShape, out Vector3 worldMin, out Vector3 worldMax)
        {
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;
            bool first = true;
            foreach (btTileData tileData in layerData.tileDatas)
            {
                btTileConfig tileConfig = this.configs.tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
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

        public BMBattle newBattle(int battleId, int mapId)
        {
            var battle = new BMBattle();
            battle.battleId = battleId;
            battle.mapId = mapId;
            battle.physicsScene = Qu3eApi.CreateScene();
            this.scripts.contactListenerScript.initListner(battle);

            Vector3 mapOffset = Vector3.zero; // to do ?

            var tilemapData = this.configs.tilemapDatas[mapId];
            foreach (btTileLayerData layerData in tilemapData.layerDatas)
            {
                Vector3 layerOffset = FVector3.ToVector3(layerData.offset);

                if (layerData.objectType == btObjectType.floor)
                {
                    btFloor floor = new btFloor();
                    floor.type = btObjectType.floor;
                    floor.id = layerData.id;
                    this.calcWorldBounds(battle, mapOffset + layerOffset, layerData, btShape.xz, out floor.worldMin, out floor.worldMax);
                    this.scripts.mainScript.addObject(battle, floor);
                }
                else if (layerData.objectType == btObjectType.stair)
                {
                    btStair stair = new btStair();
                    stair.type = btObjectType.stair;
                    stair.id = layerData.id;
                    stair.dir = layerData.stairDir;
                    this.calcWorldBounds(battle, mapOffset + layerOffset, layerData, btShape.cube, out stair.worldMin, out stair.worldMax);
                    this.scripts.mainScript.addObject(battle, stair);
                }
                else if (layerData.objectType == btObjectType.wall)
                {
                    btWall wall = new btWall();
                    wall.type = btObjectType.wall;
                    wall.id = layerData.id;
                    this.calcWorldBounds(battle, mapOffset + layerOffset, layerData, btShape.xy, out wall.worldMin, out wall.worldMax);
                    this.scripts.mainScript.addObject(battle, wall);
                }

                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = this.configs.tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    switch (tileConfig.objectType)
                    {
                        case btObjectType.box_obstacle:
                            {
                                btBoxObstacle obstacle = new btBoxObstacle();
                                obstacle.type = btObjectType.box_obstacle;
                                obstacle.id = tileData.id;
                                obstacle.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                obstacle.worldMax = obstacle.worldMin + FVector3.ToVector3(tileConfig.size);
                                obstacle.tileConfig = tileConfig;
                                obstacle.data = tileData;
                                this.scripts.mainScript.addObject(battle, obstacle);
                            }
                            break;

                        case btObjectType.tree:
                            {
                                btTree tree = new btTree();
                                tree.type = btObjectType.tree;
                                tree.id = tileData.id;
                                tree.worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                tree.worldMax = tree.worldMin + FVector3.ToVector3(tileConfig.size);
                                tree.tileConfig = tileConfig;
                                tree.data = tileData;
                                this.scripts.mainScript.addObject(battle, tree);
                            }
                            break;
                    }
                }
            }

            return battle;
        }
    }
}
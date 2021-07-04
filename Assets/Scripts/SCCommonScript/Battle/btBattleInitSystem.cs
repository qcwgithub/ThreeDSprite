using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Leopotam.Ecs;

namespace Script
{
    public class btBattleInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        // auto-injected fields: EcsWorld instance and EcsFilter.
        EcsWorld world;

        public btBattle battle;
        public btTilemapData tilemapData;
        public Dictionary<string, btTilesetConfig> tilesetConfigs;

        public void Destroy()
        {
            Qu3eApi.SceneDestroy(this.battle.physicsScene);
            this.battle.physicsScene = IntPtr.Zero;
        }

        EcsEntity createEntity(int id, btObjectType objectType, Vector3 worldMin, Vector3 worldMax, bool isWalkable, bool isObstacle)
        {
            EcsEntity entity = this.world.NewEntity();
            ref var obj_c = ref entity.Get<btObjectComponent>();
            obj_c.id = id;
            obj_c.objectType = btObjectType.floor;

            ref var physics_c = ref entity.Get<btPhysicalComponent>();
            physics_c.bodyType = q3BodyType.eStaticBody;
            physics_c.body = IntPtr.Zero;
            physics_c.worldMin = worldMin;
            physics_c.worldMax = worldMax;

            if (isWalkable)
            {
                entity.Get<btWalkableComponent>();
            }

            if (isObstacle)
            {
                entity.Get<btObstacleComponent>();
            }

            return entity;
        }

        void initEntity1(EcsEntity entity, btTileData tileData, btTileConfig tileConfig)
        {
            ref var tileData_c = ref entity.Get<btTileDataComponent>();
            tileData_c.tileData = tileData;
            tileData_c.tileConfig = tileConfig;
        }

        public void Init()
        {
            battle.tilemapData = tilemapData;
            battle.tilesetConfigs = tilesetConfigs;
            battle.physicsScene = Qu3eApi.CreateScene();
            battle.onBeginContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onBeginContact(battle, bodyA, boxA, bodyB, boxB)
                );

            battle.onEndContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onEndContact(battle, bodyA, boxA, bodyB, boxB)
                );

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

                    this.createEntity(layerData.id, btObjectType.floor, worldMin, worldMax, true, false);
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

                    this.createEntity(layerData.id, btObjectType.stair, worldMin, worldMax, true, false);
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
                    this.createEntity(layerData.id, btObjectType.wall, worldMin, worldMax, false, false);
                }

                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    switch (tileConfig.objectType)
                    {
                        case btObjectType.box_obstacle:
                            {
                                var worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                var worldMax = worldMin + FVector3.ToVector3(tileConfig.size);
                                EcsEntity entity = this.createEntity(tileData.id, btObjectType.box_obstacle, worldMin, worldMax, false, true);
                                this.initEntity1(entity, tileData, tileConfig);
                            }
                            break;

                        case btObjectType.tree:
                            {
                                var worldMin = FVector3.ToVector3(tileData.position) + mapOffset + layerOffset;
                                var worldMax = worldMin + FVector3.ToVector3(tileConfig.size);
                                EcsEntity entity = this.createEntity(tileData.id, btObjectType.tree, worldMin, worldMax, false, false);
                                this.initEntity1(entity, tileData, tileConfig);
                            }
                            break;
                    }
                }
            }

            EcsEntity characterEntity = this.world.NewEntity();
            ref btAddCharacterInfoComponent info = ref characterEntity.Get<btAddCharacterInfoComponent>();
            info.id = 10000;
            info.pos = Vector3.zero;
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
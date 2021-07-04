using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using Data;

namespace Script
{
    public struct btBodyCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        // auto-injected fields: EcsWorld instance and EcsFilter.
        EcsWorld world;
        EcsFilter<btPhysicalComponent>.Exclude<btPhysicalInitedFlagComponent> filter;

        public btBattle battle;
        public IBattleConfigs configs;
        public void Init()
        {
            this.create();
        }

        public void Run()
        {
            this.create();
        }

        void create()
        {
            foreach (var i in this.filter)
            {
                ref btPhysicalComponent physics_c = ref this.filter.Get1(i);
                Vector3 center = (physics_c.worldMin + physics_c.worldMax) / 2;
                Vector3 size = physics_c.worldMax - physics_c.worldMin;

                physics_c.body = Qu3eApi.SceneAddBody(this.battle.physicsScene, physics_c.bodyType, center.x, center.y, center.z);

                Vector3 extends = size / 2;
                Qu3eApi.BodyAddBox(physics_c.body, 0f, 0f, 0f, extends.x, extends.y, extends.z);

                this.filter.GetEntity(i).Get<btPhysicalInitedFlagComponent>();
            }
        }
    }
}

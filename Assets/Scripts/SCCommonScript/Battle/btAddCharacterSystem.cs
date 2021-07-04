using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using Data;
using System;

namespace Script
{
    public struct btAddCharacterSystem : IEcsInitSystem
    {
        public EcsWorld world;
        public btBattle battle;
        public EcsFilter<btAddCharacterInfoComponent> filter;

        public void Init()
        {
            foreach (var i in this.filter)
            {
                ref var info = ref filter.Get1(i);

                EcsEntity entity = this.world.NewEntity();
                ref var obj_c = ref entity.Get<btObjectComponent>();
                obj_c.id = info.id;
                obj_c.objectType = btObjectType.character;

                ref var char_c = ref entity.Get<btCharacterComponent>();
                char_c.pos = info.pos;


                ref var physics_c = ref entity.Get<btPhysicalComponent>();
                physics_c.bodyType = q3BodyType.eDynamicBody;
                physics_c.body = IntPtr.Zero;
                physics_c.worldMin = info.pos + new Vector3(0f, 0.4f, 0f);
                physics_c.worldMax = physics_c.worldMin + new Vector3(1f,1f,1f);
            }
        }

        // public btCharacter addCharacter(btBattle battle)
        // {
        //     btCharacter character = new btCharacter();
        //     character.battle = battle;
        //     character.type = btObjectType.character;
        //     character.id = 10000;
        //     battle.objects.Add(character.id, character);
        //     battle.characters.Add(character.id, character);
        //     character.AddToPhysicsScene();
        //     return character;
        // }
    }
}
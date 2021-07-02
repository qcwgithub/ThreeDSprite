using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class btTree : btObject
    {
        public btTileData data;
        public btTileConfig tileConfig;

        public override void AddToPhysicsScene()
        {
            Vector3 center = (worldMin + worldMax) / 2;
            Vector3 size = worldMax - worldMin;

            this.body = battle.AddBody(this, q3BodyType.eStaticBody, center);
            this.battle.AddBox(this.body, Vector3.zero, size / 2);
        }
    }
}
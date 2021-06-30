using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class btWall : btObject, btIObstacle
    {
        public Vector3 worldMin;
        public Vector3 worldMax;
        public btWall(btBattle scene, int id, Vector3 worldMin, Vector3 worldMax) : base(scene, id)
        {
            this.worldMin = worldMin;
            this.worldMax = worldMax;
        }
        public override btObjectType Type { get { return btObjectType.wall; } }

        //public override int Priority { get { return 1; } }

        public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
        {
            return false;
            Vector3 to = from + delta;
            if (to.x < worldMin.x || to.x > worldMax.x || to.z < worldMin.z || to.z > worldMax.z)
            {
                return false;
            }

            bool ret = false;
            if (from.x <= worldMin.x && delta.x > 0 && to.x > worldMin.x)
            {
                delta.x = 0;
                ret = true;
            }
            else if (from.x >= worldMax.x && delta.x < 0 && to.x < worldMax.x)
            {
                delta.x = 0;
                ret = true;
            }
            if (from.z <= worldMin.z && delta.z > 0 && to.z > worldMin.z)
            {
                delta.z = 0;
                ret = true;
            }
            else if (from.z >= worldMax.z && delta.z < 0 && to.z < worldMax.z)
            {
                delta.z = 0;
                ret = true;
            }
            return ret;
        }

        // public Vector3 RandomPos()
        // {
        //     LFloorData data = this.Data;
        //     return new Vector3(UnityEngine.Random.Range(data.Min.x, data.Max.x), data.Y, UnityEngine.Random.Range(data.Min.z, data.Max.z));
        // }

        public override void AddToPhysicsScene()
        {
            Vector3 center = (worldMin + worldMax) / 2;
            Vector3 size = worldMax - worldMin;

            this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
            scene.AddBox(this.body, Vector3.zero, size / 2);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class btFloor : btObject, btIWalkable
    {
        public float y;

        //public override int Priority { get { return 1; } }

        protected bool CheckXZOutOfRange(Vector3 pos)
        {
            bool outOfRange = false;
            if (pos.x < worldMin.x)
            {
                pos.x = worldMin.x;
                outOfRange = true;
            }
            else if (pos.x > worldMax.x)
            {
                pos.x = worldMax.x;
                outOfRange = true;
            }

            if (pos.z < worldMin.z)
            {
                pos.z = worldMin.z;
                outOfRange = true;
            }
            else if (pos.z > worldMax.z)
            {
                pos.z = worldMax.z;
                outOfRange = true;
            }
            return outOfRange;
        }

        // public Vector3 RandomPos()
        // {
        //     LFloorData data = this.Data;
        //     return new Vector3(UnityEngine.Random.Range(data.Min.x, data.Max.x), data.Y, UnityEngine.Random.Range(data.Min.z, data.Max.z));
        // }

        public PredictMoveResult PredictMove(Vector3 from, Vector3 delta)
        {
            PredictMoveResult result = default;
            Vector3 to = from + delta;
            if (this.CheckXZOutOfRange(to))
            {
                result.OutOfRange = true;
                return result;
            }

            result.Y = this.y;
            return result;
        }

        public bool CanAccept(Vector3 from, Vector3 delta)
        {
            Vector3 to = from + delta;
            if (this.CheckXZOutOfRange(to))
            {
                return false;
            }
            if (delta.y < 0 && from.y > this.y && to.y <= this.y)
            {
                return true;
            }
            if (Mathf.Abs(this.y - to.y) > 0.1f)
            {
                return false;
            }
            return true;
        }

        public override void AddToPhysicsScene()
        {
            Vector3 center = (worldMin + worldMax) / 2;
            Vector3 size = worldMax - worldMin;

            this.body = battle.AddBody(this, q3BodyType.eStaticBody, center);
            battle.AddBox(this.body, Vector3.zero, size / 2);
        }
    }
}
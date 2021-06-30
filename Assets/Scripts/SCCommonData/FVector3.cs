using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public struct FVector3
    {
        public float x, y, z;

        public static FVector3 FromVector3(Vector3 v)
        {
            FVector3 lv = new FVector3();
            lv.x = v.x;
            lv.y = v.y;
            lv.z = v.z;
            return lv;
        }
        public static Vector3 ToVector3(FVector3 lv)
        {
            return new Vector3(lv.x, lv.y, lv.z);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LVector3
{
    public float x, y, z;

    public static LVector3 FromVector3(Vector3 v)
    {
        LVector3 lv = new LVector3();
        lv.x = v.x;
        lv.y = v.y;
        lv.z = v.z;
        return lv;
    }
    public static Vector3 ToVector3(LVector3 lv)
    {
        return new Vector3(lv.x, lv.y, lv.z);
    }
}

public class LFloorData
{
    public int Id;
    public LVector3 Min;
    public LVector3 Max;
    public float Y;
}

public class LStairData
{
    public int Id;
    public StairDir Dir;
    public LVector3 Min;
    public LVector3 Max;
}

public class LBoxObstacleData
{
    public int Id;
    public LVector3 Min;
    public LVector3 Max;
    public bool Walkable;
}

public class LMapData
{
    public int Id;
    public LFloorData[] Floors;
    public LStairData[] Stairs;
    public LBoxObstacleData[] BoxObstacles;
}

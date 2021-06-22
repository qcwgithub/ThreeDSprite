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

public class btFloorData
{
    public int Id;
    public LVector3 Min;
    public LVector3 Max;
    public float Y;
}

public class btStairData
{
    public int Id;
    public StairDir Dir;
    public LVector3 Min;
    public LVector3 Max;
}

public class btBoxObstacleData
{
    public int Id;
    public LVector3 Min;
    public LVector3 Max;
}

public class btSceneData
{
    public int Id;
    public btFloorData[] Floors;
    public btStairData[] Stairs;
    public btBoxObstacleData[] BoxObstacles;
    public btTreeData[] Trees;
}

public class btTreeData
{
    public int Id;
    public LVector3 Min;
    public LVector3 Max;
}

public enum btThingShape
{
    cube,
    xy,
    xz,
}

public abstract class btThingConfig
{
    public abstract btThingShape getShape();
}

public class btThingConfigCube : btThingConfig
{
    public override btThingShape getShape() { return btThingShape.cube; }
    public LVector3 size;
}

public class btThingConfigXY : btThingConfig
{
    public override btThingShape getShape() { return btThingShape.xy; }
    public LVector3 size;
}

public class btThingConfigXZ : btThingConfig
{
    public override btThingShape getShape() { return btThingShape.xz; }
    public LVector3 size;
}

public class btTilesetConfig
{
    public string path;
    public Dictionary<string, btThingConfigCube> cubes;
    public Dictionary<string, btThingConfigXY> xys;
    public Dictionary<string, btThingConfigXZ> xzs;
}

public class btMapConfig
{
    public HashSet<string> tilesets;
    
}
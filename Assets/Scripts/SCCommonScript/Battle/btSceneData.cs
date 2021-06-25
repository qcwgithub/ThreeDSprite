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

public class btObjectData
{
    public int id;
}

public class btSingleObjectData : btObjectData
{
    public int configId;
}

public class btGroupObjectData : btObjectData
{
    public LVector3 min;
    public LVector3 max;
}

public class btFloorData : btGroupObjectData
{
    public float y;
}

public class btStairData : btGroupObjectData
{
    public StairDir dir;
}

public class btBoxObstacleData : btSingleObjectData
{
    public LVector3 min; // todo: delete
    public LVector3 max; // todo: delete
}

public class btTreeData : btSingleObjectData
{
    public LVector3 min; // todo: delete
    public LVector3 max; // todo: delete
}

public class btSceneData
{
    public int id;
    public btFloorData[] floors;
    public btStairData[] stairs;
    public btBoxObstacleData[] boxObstacles;
    public btTreeData[] trees;
}

public enum btThingShape
{
    cube,
    xy,
    xz,
}

public class btThingConfig
{
    public btThingShape shape;
    public btObjectType objectType;
    public string spriteName;
    public LVector3 pixelSize;
}

// .tsx
public class btTilesetConfig
{
    // key = tile id
    public Dictionary<int, btThingConfig> tiles;
}

public class btTileLayerConfig
{
    public int id;
    public string name;
    public string type;
    public bool visible;

    // 在 layer 上摆放的一个东西
    public class AThing
    {
        // 东西是啥
        public string tileset;
        public int tileId; // tile id in tileset

        // 坐标是啥，左下角
        public int pixelX; // pixelX / PixelsPerUnit = x
        public int pixelY;
        public int pixelZ;
    }

    public List<AThing> things;
}

// .tmx
public class btTilemapConfig
{
    public int pixelWidth;
    public int pixelHeight;
    public List<btTileLayerConfig> layers;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IVector3
{
    public int x, y, z;

    // public static IVector3 FromVector3(Vector3 v)
    // {
    //     IVector3 lv = new IVector3();
    //     lv.x = v.x;
    //     lv.y = v.y;
    //     lv.z = v.z;
    //     return lv;
    // }
    // public static Vector3 ToVector3(FVector3 lv)
    // {
    //     return new Vector3(lv.x, lv.y, lv.z);
    // }
}

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

public class btObjectData
{
    public int id;
}

public class btSingleObjectData : btObjectData
{
    public string tileset; // no ext
    public int tileId;
}

public class btGroupObjectData : btObjectData
{
    public FVector3 min;
    public FVector3 max;
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
    public FVector3 min; // todo: delete
    public FVector3 max; // todo: delete
}

public class btTreeData : btSingleObjectData
{
    public FVector3 min; // todo: delete
    public FVector3 max; // todo: delete
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
    public FVector3 pixelSize;
}

// .tsx
public class btTilesetConfig
{
    // key = tile id
    public Dictionary<int, btThingConfig> tiles;
}

// 在 layer 上摆放的一个东西
public class btThingData
{
    public int id;
    
    // 东西是啥
    public string tileset;
    public int tileId; // tile id in tileset

    // 坐标是啥，左下角
    public IVector3 pixelPosition; // pixelX / PixelsPerUnit = x
}

public class btTileLayerData
{
    public int id;
    public string name;
    public int pixelY;
    public btObjectType objectType;

    public List<btThingData> thingDatas;

    // when objectType == stair
    public StairDir stairDir;
}

// .tmx
public class btTilemapData
{
    public int pixelWidth;
    public int pixelHeight;
    public List<btTileLayerData> layerDatas;
}
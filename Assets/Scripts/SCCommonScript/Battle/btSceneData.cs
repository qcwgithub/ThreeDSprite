using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

// 在 layer 上摆放的一个东西
public class btTileData
{
    public int id;

    // 东西是啥
    public string tileset;
    public int tileId; // tile id in tileset

    // 坐标是啥，左下角
    // transform.position
    public FVector3 position;
}

public class btTileLayerData
{
    public int id;
    public string name;
    public FVector3 offset;
    public btObjectType objectType;

    public List<btTileData> tileDatas;

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
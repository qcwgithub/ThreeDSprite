using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledConstants
{
    public const string tilemap_ext = ".tmx";
    public const string tileset_ext = ".tsx";
    public const string tilemap_ext_filter = "*.tmx";
    public const string tileset_ext_filter = "*.tsx";
}

public class TilemapPropertyKey
{
    // tilemap 导入到 Unity 后会变成地图原点的格子坐标
    // 单位是格子数，可能是负数
    // 这个配置是必须的
    public const string x_origin = "x_origin";
    public const string y_origin = "y_origin";
    public const string z_origin = "z_origin";
}

public class TilesetPropertyKey
{
    // 在 tileset 中配置，表示所有包含的 tile 都用这个 shape
    // 值对应于枚举 btShape
    // 此配置不是必须的
    public const string tile_shape = "tile_shape";
    public const string tile_object_type = "tile_object_type";
}

public class TilePropertyKey
{
    // 在 tile 中配置，表示这个 tile 的 shape
    // 值对应于枚举 btShape
    // 如果父 tileset 中有配置 each_shape，则 tile 中可以不配置；否则必须配置
    public const string shape = "shape";

    // 配置在 tile 中，表示每个 tile 都是一个单独的对象
    // 值对应于枚举 btObjectType
    // 这个配置不是必须的
    public const string object_type = "object_type";

    // cube 类型的 tile 需要配置一个 y 的高度（上面部分是 z）
    // 单位是像素，从下到分隔点的高度
    // cube 类型的 tile 必须配置，否则报错
    public const string cube_y_height = "cube_y_height";
}

public class LayerPropertyKey
{
    // 配置在 layer 中，表示一整个 layer 组合成一个对象
    // 值对应于枚举 btObjectType
    // 这个配置不是必须的
    public const string object_type = "object_type";

    // 单位是格子
    public const string layer_y = "layer_y";

    public const string stair_dir = "stair_dir";
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledConstants
{
    public const string tilemap_ext = ".tmx";
    public const string tileset_ext = ".tsx";
    public const string tilemap_ext_filter = "*.tmx";
    public const string tileset_ext_filter = "*.tsx";
    public const string property_shape = "shape";
    public const string property_x_origin = "x_origin";
    public const string property_y_origin = "y_origin";
    public const string property_y_height = "y_height";
}

public enum ETiledProperty
{
    // tileset or tile
    shape,

    // tilemap
    x_origin,

    // tilemap
    y_origin,

    // tile.cube
    y_height,

    // belong to: layer | tile
    object_type,
}
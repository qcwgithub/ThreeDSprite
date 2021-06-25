using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiledCS;
using System.Linq;

public static class TiledCSExtension
{
    static string findProperty(this TiledProperty[] properties, string key)
    {
        if (properties == null) return null;
        TiledProperty p = properties.FirstOrDefault(p_ => p_.name == key);
        if (p == null) return null;
        return p.value;
    }

    public static int findInt(this TiledProperty[] properties, string key, int default_)
    {
        string p = findProperty(properties, key);
        if (p == null) return default_;
        return int.Parse(p);
    }

    public static string findString(this TiledProperty[] properties, string key, string default_)
    {
        string p = findProperty(properties, key);
        if (p == null) return default_;
        return p;
    }

    public static bool findEnum<T>(this TiledProperty[] properties, string key, out T result) where T : Enum
    {
        result = default(T);
        string p = findProperty(properties, key);
        if (p == null) return false;
        result = (T) Enum.Parse(typeof(T), p);
        return true;
    }
    
    public static T findEnum<T>(this TiledProperty[] properties, string key, T default_) where T : Enum
    {
        string p = findProperty(properties, key);
        if (p == null) return default_;
        return (T) Enum.Parse(typeof(T), p);
    }

    public static TiledMapTileset mapDataIdToTilesetInfo(this TiledMap tilemap, int dataId)
    {
        for (int i = 0; i < tilemap.Tilesets.Length; i++)
        {
            TiledMapTileset ts = tilemap.Tilesets[i];
            if (dataId >= ts.firstgid && (i == tilemap.Tilesets.Length - 1 || dataId < tilemap.Tilesets[i + 1].firstgid))
            {
                return ts;           
            }
        }
        return null;
    }
}

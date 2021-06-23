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
}

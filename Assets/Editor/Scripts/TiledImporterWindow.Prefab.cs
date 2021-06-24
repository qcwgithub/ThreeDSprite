using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TiledCS;
using Data;
using Newtonsoft.Json;

public partial class TiledImporterWindow
{
    bool ImportPrefabPrepare(string fileName, out btTilemapConfig mapConfig, out Dictionary<string, btTilesetConfig> tilesetConfigs)
    {
        mapConfig = null;
        tilesetConfigs = null;

        ////
        var tilemapPath = this.importedDir + "/" + fileName;
        TextAsset mapAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilemapPath);
        if (mapAsset == null)
        {
            Debug.LogError(tilemapPath + " not imported");
            return false;
        }
        mapConfig = JsonUtils.FromJson<btTilemapConfig>(mapAsset.text);

        ////
        tilesetConfigs = new Dictionary<string, btTilesetConfig>();
        for (int i = 0; i < mapConfig.layers.Count; i++)
        {
            btTileLayerConfig layerConfig = mapConfig.layers[i];

            for (int j = 0; j < layerConfig.things.Count; j++)
            {
                btTileLayerConfig.AThing aThing = layerConfig.things[j];
                // string key = Path.GetFileNameWithoutExtension(aThing.tileset);

                btTilesetConfig tilesetConfig;
                if (!tilesetConfigs.TryGetValue(aThing.tileset, out tilesetConfig))
                {
                    string tilesetPath = this.importedDir + "/" + aThing.tileset + ".json";
                    TextAsset tilesetAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilesetPath);
                    if (tilesetAsset == null)
                    {
                        Debug.LogError(tilesetPath + " not imported");
                        break;
                    }
                    tilesetConfig = JsonUtils.FromJson<btTilesetConfig>(tilesetAsset.text);
                    tilesetConfigs.Add(aThing.tileset, tilesetConfig);
                }
            }
        }
        return true;
    }

    btThingConfig findbtThingConfig(btTilesetConfig tilesetConfig, int tileId)
    {
        var t = tilesetConfig;
        if (t.cubes != null && t.cubes.ContainsKey(tileId))
        {
            return t.cubes[tileId];
        }
        if (t.xys != null && t.xys.ContainsKey(tileId))
        {
            return t.xys[tileId];
        }
        if (t.xzs != null && t.xzs.ContainsKey(tileId))
        {
            return t.xzs[tileId];
        }
        return null;
    }

    const float SQRT2 = 1.414213562373095f;
    void ImportPrefab(string fileName)
    {
        btTilemapConfig mapConfig;
        // key = xxx.tsx (no extension)
        Dictionary<string, btTilesetConfig> tilesetConfigs;
        if (!this.ImportPrefabPrepare(fileName, 
            out mapConfig, 
            out tilesetConfigs))
        {
            return;
        }

        string name = "__" + fileName + "__";
        var existed = GameObject.Find(name);
        if (existed)
        {
            DestroyImmediate(existed);
        }
        var mapTrans = new GameObject(name).transform;

        for (int i = 0; i < mapConfig.layers.Count; i++)
        {
            btTileLayerConfig layerConfig = mapConfig.layers[i];
            var layerTrans = new GameObject(layerConfig.name).transform;
            layerTrans.SetParent(mapTrans);

            for (int j = 0; j < layerConfig.things.Count; j++)
            {
                btTileLayerConfig.AThing aThing = layerConfig.things[j];
                btTilesetConfig tilesetConfig = tilesetConfigs[aThing.tileset];
                string atlasName = Path.GetFileNameWithoutExtension(aThing.tileset);
                btThingConfig thingConfig = this.findbtThingConfig(tilesetConfig, aThing.tileId);
                if (thingConfig == null)
                {
                    Debug.LogError(string.Format("layer({0}) tileId({1}) btThingConfig is null", layerConfig.name, aThing.tileId));
                    return;
                }

                var thingGo = new GameObject(string.Format("{0}_{1}", thingConfig.getShape(), thingConfig.spriteName));
                var thingTrans = thingGo.transform;
                thingTrans.rotation = Quaternion.Euler(45f, 0f, 0f);
                thingTrans.SetParent(layerTrans);

                thingTrans.position = new Vector3(aThing.pixelX / 100f, 0f, aThing.pixelY * SQRT2 / 100f);

                var renderer = thingGo.AddComponent<SpriteRenderer>();
                renderer.spriteSortPoint = SpriteSortPoint.Pivot;
                var spritePath = this.atlasDir + "/" + atlasName + "/" + thingConfig.spriteName + ".png";
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite == null)
                {
                    var abc = AssetDatabase.LoadAllAssetsAtPath(spritePath);
                    Debug.LogError("sprite not found at: " + spritePath);
                }
                renderer.sprite = sprite;
            }
        }

    }
}

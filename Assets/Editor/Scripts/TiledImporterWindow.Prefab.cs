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
    // SETTINGS
    const float sqrt2 = 1.414213562373095f;
    Quaternion sprite_rotation = Quaternion.Euler(45f, 0f, 0f);
    const float pixels_per_unit = 100f;
    const SpriteSortPoint sprite_sort_point = SpriteSortPoint.Pivot;
    Vector2 sprite_pivot_cube = new Vector2(0.5f, 0f);
    Vector2 sprite_pivot_xy = new Vector2(0.5f, 0f);
    Vector2 sprite_pivot_xz = new Vector2(0.5f, 1f);

    bool ImportPrefabPrepare(string fileName, out btTilemapConfig mapConfig, out Dictionary<string, btTilesetConfig> tilesetConfigs)
    {
        mapConfig = null;
        tilesetConfigs = null;

        // .tmx
        var tilemapPath = this.importedDir + "/" + fileName;
        TextAsset mapAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilemapPath);
        if (mapAsset == null)
        {
            Debug.LogError(tilemapPath + " not imported");
            return false;
        }
        mapConfig = JsonUtils.FromJson<btTilemapConfig>(mapAsset.text);

        // .tsx
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

    Sprite loadSprite(ref Dictionary<string, Sprite> sprites, string atlasName, string spriteName)
    {
        var spritePath = this.atlasDir + "/" + atlasName + "/" + spriteName + ".png";
        Sprite sprite;
        if (sprites.TryGetValue(spritePath, out sprite))
        {
            return sprite;
        }

        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        sprites.Add(spritePath, sprite);

        if (sprite == null)
        {
            if (!File.Exists(spritePath))
            {
                Debug.LogError("file not exist: " + spritePath);
            }
            else
            {
                Debug.LogError("sprite not found at: " + spritePath);
            }
            return sprite;
        }

        return sprite;
    }

    Vector2 getCorrectSpritePivot(btThingShape shape)
    {
        Vector2 expected;
        switch (shape)
        {
            case btThingShape.cube:
                expected = sprite_pivot_cube;
                break;
            case btThingShape.xy:
                expected = sprite_pivot_xy;
                break;
            case btThingShape.xz:
            default:
                expected = sprite_pivot_xz;
                break;
        }
        return expected;
    }

    Vector2 getSpritePivot01(Sprite sprite)
    {
        Vector2 pivot = sprite.pivot;
        Rect rect = sprite.rect;
        return new Vector2(pivot.x / rect.width, pivot.y / rect.height);
    }

    // 在 tiled 中对齐是左下角
    Vector3 calcSpritePosition(int pixelX, int pixelY, int pixelZ, Sprite sprite)
    {
        Vector2 pivot = this.getSpritePivot01(sprite);
        Rect rect = sprite.rect;
        float px = pixelX + pivot.x * rect.width;
        float py = pixelY * sqrt2; // todo
        float pz = (pixelZ + pivot.y * rect.height) * sqrt2;
        Vector3 pos = new Vector3(px / pixels_per_unit, py / pixels_per_unit, pz / pixels_per_unit);
        return pos;
    }

    void ImportPrefab(string fileName)
    {
        btTilemapConfig mapConfig;
        // key = xxx.tsx (no extension)
        Dictionary<string, btTilesetConfig> tilesetConfigs;
        if (!this.ImportPrefabPrepare(fileName, out mapConfig, out tilesetConfigs))
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

        var sprites = new Dictionary<string, Sprite>();

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

                // load sprite
                var sprite = this.loadSprite(ref sprites, atlasName, thingConfig.spriteName);

                if (sprite != null)
                {
                    Vector2 correctPivot = this.getCorrectSpritePivot(thingConfig.getShape());
                    Vector2 pivot = this.getSpritePivot01(sprite);
                    if (pivot != correctPivot)
                    {
                        Debug.LogError(string.Format("sprite '{0}/{1}' pivot is {2}, should be {3}", atlasName, thingConfig.spriteName, pivot, correctPivot));
                    }

                    var thingGo = new GameObject(string.Format("{0}_{1}", thingConfig.getShape(), thingConfig.spriteName));
                    var thingTrans = thingGo.transform;
                    thingTrans.rotation = sprite_rotation;
                    thingTrans.SetParent(layerTrans);

                    // set position
                    thingTrans.position = this.calcSpritePosition(aThing.pixelX, aThing.pixelY, aThing.pixelZ, sprite);

                    // add sprite renderer
                    var renderer = thingGo.AddComponent<SpriteRenderer>();
                    renderer.spriteSortPoint = sprite_sort_point;
                    renderer.sprite = sprite;
                }
            }
        }
    }
}

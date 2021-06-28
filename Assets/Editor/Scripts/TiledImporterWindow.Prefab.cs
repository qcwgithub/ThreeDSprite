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

    bool ImportPrefabPrepare(string fileName, out btTilemapData mapData, out Dictionary<string, btTilesetConfig> tilesetConfigs)
    {
        mapData = null;
        tilesetConfigs = null;

        // .tmx
        var tilemapPath = this.importedDir + "/" + fileName;
        TextAsset mapAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilemapPath);
        if (mapAsset == null)
        {
            Debug.LogError(tilemapPath + " not imported");
            return false;
        }
        mapData = JsonUtils.FromJson<btTilemapData>(mapAsset.text);

        // .tsx
        tilesetConfigs = new Dictionary<string, btTilesetConfig>();
        for (int i = 0; i < mapData.layerDatas.Count; i++)
        {
            btTileLayerData layerData = mapData.layerDatas[i];

            for (int j = 0; j < layerData.thingDatas.Count; j++)
            {
                btTileData thingData = layerData.thingDatas[j];
                // string key = Path.GetFileNameWithoutExtension(aThing.tileset);

                btTilesetConfig tilesetConfig;
                if (!tilesetConfigs.TryGetValue(thingData.tileset, out tilesetConfig))
                {
                    string tilesetPath = this.importedDir + "/" + thingData.tileset + ".json";
                    TextAsset tilesetAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilesetPath);
                    if (tilesetAsset == null)
                    {
                        Debug.LogError(tilesetPath + " not imported");
                        break;
                    }
                    tilesetConfig = JsonUtils.FromJson<btTilesetConfig>(tilesetAsset.text);
                    tilesetConfigs.Add(thingData.tileset, tilesetConfig);
                }
            }
        }
        return true;
    }

    btTileConfig findbtThingConfig(btTilesetConfig tilesetConfig, int tileId)
    {
        var t = tilesetConfig;
        if (t.tiles.ContainsKey(tileId))
        {
            return t.tiles[tileId];
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

    Vector2 getCorrectSpritePivot(btShape shape)
    {
        Vector2 expected;
        switch (shape)
        {
            case btShape.cube:
                expected = btConstants.sprite_pivot_cube;
                break;
            case btShape.xy:
                expected = btConstants.sprite_pivot_xy;
                break;
            case btShape.xz:
            default:
                expected = btConstants.sprite_pivot_xz;
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
    Vector3 calcSpritePosition(FVector3 position, btTileConfig thingConfig)
    {
        Vector2 pivot = this.getCorrectSpritePivot(thingConfig.shape);
        float px = position.x + pivot.x * thingConfig.size.x;
        float py = position.y;
        float pz = position.z + pivot.y * thingConfig.size.z;
        Vector3 pos = new Vector3(px, py, pz);
        return pos;
    }

    void ImportThingPrefab(btTileLayerData layerData, Transform layerTrans,
        Dictionary<string, btTilesetConfig> tilesetConfigs,
        ref Dictionary<string, Sprite> sprites,
        btTileData thingData)
    {
        btTilesetConfig tilesetConfig = tilesetConfigs[thingData.tileset];
        string atlasName = Path.GetFileNameWithoutExtension(thingData.tileset);
        btTileConfig thingConfig = this.findbtThingConfig(tilesetConfig, thingData.tileId);
        if (thingConfig == null)
        {
            Debug.LogError(string.Format("layer({0}) tileId({1}) btThingConfig is null", layerData.name, thingData.tileId));
            return;
        }

        // load sprite
        var sprite = this.loadSprite(ref sprites, atlasName, thingConfig.spriteName);

        if (sprite != null)
        {
            Vector2 correctPivot = this.getCorrectSpritePivot(thingConfig.shape);
            Vector2 pivot = this.getSpritePivot01(sprite);
            if (pivot != correctPivot)
            {
                Debug.LogError(string.Format("sprite '{0}/{1}' pivot is {2}, should be {3}", atlasName, thingConfig.spriteName, pivot, correctPivot));
            }

            var thingGo = new GameObject(string.Format("{0} (id:{1})", thingConfig.spriteName, thingData.id));
            var thingTrans = thingGo.transform;
            thingTrans.rotation = btConstants.sprite_rotation;
            thingTrans.SetParent(layerTrans);

            // set position
            thingTrans.position = this.calcSpritePosition(thingData.position, thingConfig);

            // add sprite renderer
            var renderer = thingGo.AddComponent<SpriteRenderer>();
            renderer.spriteSortPoint = btConstants.sprite_sort_point;
            renderer.sprite = sprite;

            //--------------------------------------------------------
            switch (thingConfig.objectType)
            {
                case btObjectType.none:
                    break;
                case btObjectType.box_obstacle:
                    {
                        var obj = thingGo.AddComponent<BtBoxObstacle>();
                        obj.Id = thingData.id;
                    }
                    break;
                case btObjectType.tree:
                    {
                        var obj = thingGo.AddComponent<BtTree>();
                        obj.Id = thingData.id;
                    }
                    break;
                default:
                    throw new Exception("unhandled thingConfig.objectType: " + thingConfig.objectType);
            }
        }
    }

    void ImportLayerPrefab(Transform mapTrans,
        Dictionary<string, btTilesetConfig> tilesetConfigs,
        ref Dictionary<string, Sprite> sprites,
        btTileLayerData layerData)
    {
        var layerGo = new GameObject(string.Format("{0} (id:{1})", layerData.name, layerData.id));
        var layerTrans = layerGo.transform;
        layerTrans.SetParent(mapTrans);

        for (int j = 0; j < layerData.thingDatas.Count; j++)
        {
            btTileData thingData = layerData.thingDatas[j];
            this.ImportThingPrefab(layerData, layerTrans, tilesetConfigs, ref sprites, thingData);
        }

        switch (layerData.objectType)
        {
            case btObjectType.none:
                break;
            case btObjectType.floor:
                {
                    var obj = layerGo.AddComponent<BtFloor>();
                    obj.Id = layerData.id;
                }
                break;

            case btObjectType.stair:
                {
                    var obj = layerGo.AddComponent<BtStair>();
                    obj.Id = layerData.id;
                }
                break;

            default:
                throw new Exception("unhandled layerData.objectType: " + layerData.objectType);
        }
    }

    void ImportMapPrefab(string fileName)
    {
        btTilemapData mapData;
        // key = xxx.tsx (no extension)
        Dictionary<string, btTilesetConfig> tilesetConfigs;
        if (!this.ImportPrefabPrepare(fileName, out mapData, out tilesetConfigs))
        {
            return;
        }

        string name = Path.GetFileNameWithoutExtension(fileName);
        if (name.Contains('.'))
        {
            name = Path.GetFileNameWithoutExtension(name);
        }
        var existed = GameObject.Find(name);
        if (existed)
        {
            DestroyImmediate(existed);
        }

        var mapGo = new GameObject(name);
        var mapTrans = mapGo.transform;

        var sprites = new Dictionary<string, Sprite>();

        for (int i = 0; i < mapData.layerDatas.Count; i++)
        {
            btTileLayerData layerData = mapData.layerDatas[i];
            this.ImportLayerPrefab(mapTrans, tilesetConfigs, ref sprites, layerData);
        }

        //---
        mapGo.AddComponent<BtScene>();

        bool success;
        string prefabPath = this.importedDir + "/" + name + ".prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(mapGo, prefabPath, out success);
        if (!success)
        {
            Debug.LogError(string.Format("Save to prefab failed, prefab path: {0}", prefabPath));
        }
        else
        {
            Debug.Log(string.Format("Save to prefab succeeded, prefab path: {0}", prefabPath), context: prefab);
            DestroyImmediate(mapGo);
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using TiledCS;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public partial class TiledImporterWindow
{
    void ParseExtraLayerDataFields(TiledLayer layer, btTileLayerData layerData)
    {
        switch (layerData.objectType)
        {
            case btObjectType.stair:
                if (!layer.Properties.findEnum<StairDir>(LayerPropertyKey.stair_dir, out layerData.stairDir))
                {
                    throw new Exception(LayerPropertyKey.stair_dir + " not defined");
                }
                break;
        }
    }

    btTileLayerData ParseLayer(TiledMap map, Vector3Int origin, TiledLayer layer)
    {
        btTileLayerData layerData = new btTileLayerData();
        layerData.id = this.getNextObjectId();
        layerData.name = layer.name;
        layerData.pixelY = layer.Properties.findInt(LayerPropertyKey.layer_y, 0) * map.TileHeight;
        //layerConfig.type = layer.type;
        layerData.objectType = layer.Properties.findEnum<btObjectType>(LayerPropertyKey.object_type, btObjectType.none);
        this.ParseExtraLayerDataFields(layer, layerData);
        layerData.thingDatas = new List<btThingData>();

        for (int j = 0; j < layer.data.Length; j++)
        {
            int dataId = layer.data[j];
            if (dataId == 0)
            {
                continue;
            }

            TiledMapTileset ts = map.mapDataIdToTilesetInfo(dataId);
            if (ts == null)
            {
                throw new Exception("data id not valid: " + dataId);
            }

            int x = j % map.Width;
            int z = j / map.Width;

            IVector3 pixelPos;
            pixelPos.x = (x - origin.x) * map.TileWidth;
            pixelPos.y = 0;//(y - origin.y) * map.TileHeight; // todo get from layer.properties
                           // 在 tiled 中 y 轴是向下的（就是这里的 pixelZ）
            pixelPos.z = -((z - origin.z) * map.TileHeight);

            btThingData thingData = new btThingData();
            thingData.id = this.getNextObjectId();
            thingData.tileset = ts.source;
            thingData.tileId = dataId - ts.firstgid;
            thingData.pixelPosition = pixelPos;
            layerData.thingDatas.Add(thingData);
        }

        return layerData;
    }

    int objectId = 1;
    int getNextObjectId()
    {
        return this.objectId++;
    }

    // .tmx -> .json
    // 变成与 tiled 编辑器无关的格式
    void ImportTilemap(string fileName)
    {
        this.objectId = 1;

        var filePath = tiledDir + "/" + fileName;
        var map = new TiledMap(filePath);
        Debug.Log(string.Format("map size: {0} x {1}", map.Width, map.Height));
        Debug.Log(string.Format("map tile size: {0} x {1}", map.TileWidth, map.TileHeight));

        Vector3Int origin = new Vector3Int(
            map.Properties.findInt(TilemapPropertyKey.x_origin, -1),
            map.Properties.findInt(TilemapPropertyKey.y_origin, -1),
            map.Properties.findInt(TilemapPropertyKey.z_origin, -1));

        if (origin.x == -1 || origin.y == -1 || origin.z == -1)
        {
            throw new Exception("x_origin || y_origin || z_origin not defined");
        }

        /*
                List<TextAsset> tilesetAssets = new List<TextAsset>();
                for (int i = 0; i < map.Tilesets.Length; i++)
                {
                    TiledMapTileset tileset = map.Tilesets[i];
                    string tilesetPath = this.importedDirectory + "/" + tileset.source + ".json";
                    TextAsset tilesetAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilesetPath);
                    if (tilesetAsset == null)
                    {
                        Debug.LogError(tilesetPath + " not imported");
                        break;
                    }

                    Debug.Log("loaded tileset: " + tilesetPath);
                    tilesetAssets.Add(tilesetAsset);
                }
        */
        btTilemapData mapData = new btTilemapData();
        mapData.pixelWidth = map.Width * map.TileWidth;
        mapData.pixelHeight = map.Height * map.TileHeight;
        mapData.layerDatas = new List<btTileLayerData>();

        for (int i = 0; i < map.Layers.Length; i++)
        {
            TiledLayer layer = map.Layers[i];
            if (layer.data == null)
            {
                // not a tile layer
                continue;
            }

            // if (!layer.visible)
            // {
            //     continue;
            // }

            btTileLayerData layerData = this.ParseLayer(map, origin, layer);
            mapData.layerDatas.Add(layerData);
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(mapData, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using TiledCS;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public partial class TiledImporterWindow
{
    // .tmx -> .json
    void ImportTilemap(string fileName)
    {
        var filePath = tiledDir + "/" + fileName;
        var map = new TiledMap(filePath);
        Debug.Log(string.Format("map size: {0} x {1}", map.Width, map.Height));
        Debug.Log(string.Format("map tile size: {0} x {1}", map.TileWidth, map.TileHeight));

        Vector3Int origin = new Vector3Int(
            map.Properties.findInt("x_origin", -1), 
            map.Properties.findInt("y_origin", -1), 
            map.Properties.findInt("z_origin", -1));

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
        btTilemapConfig mapConfig = new btTilemapConfig();
        mapConfig.pixelWidth = map.Width * map.TileWidth;
        mapConfig.pixelHeight = map.Height * map.TileHeight;
        mapConfig.layers = new List<btTileLayerConfig>();

        for (int i = 0; i < map.Layers.Length; i++)
        {
            TiledLayer layer = map.Layers[i];
            if (layer.data == null)
            {
                // not a tile layer
                continue;
            }

            btTileLayerConfig layerConfig = new btTileLayerConfig();
            layerConfig.id = layer.id;
            layerConfig.name = layer.name;
            layerConfig.type = layer.type;
            layerConfig.visible = layer.visible;
            layerConfig.things = new List<btTileLayerConfig.AThing>();
            mapConfig.layers.Add(layerConfig);

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

                int pixelX = (x - origin.x) * map.TileWidth;
                int pixelY = 0;//(y - origin.y) * map.TileHeight;
                int pixelZ = (z - origin.z) * map.TileHeight;

                btTileLayerConfig.AThing aThing = new btTileLayerConfig.AThing();
                aThing.tileset = ts.source;
                aThing.tileId = dataId - ts.firstgid;
                aThing.pixelX = pixelX;
                aThing.pixelY = pixelY; // todo get from layer.properties
                // 在 tiled 中 y 轴是向下的（就是这里的 pixelZ）
                aThing.pixelZ = -pixelZ;
                layerConfig.things.Add(aThing);
            }
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(mapConfig, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

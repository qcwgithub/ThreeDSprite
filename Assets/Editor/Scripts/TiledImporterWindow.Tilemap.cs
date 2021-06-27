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

    btTileLayerData ParseLayer(TiledMap map, TiledLayer layer)
    {
        btTileLayerData layerData = new btTileLayerData();
        layerData.id = this.getNextObjectId();
        layerData.name = layer.name;
        layerData.y = this.coordConverter.ConvertCoordY(layer.Properties.findInt(LayerPropertyKey.layer_y, 0));
        //layerConfig.type = layer.type;
        layerData.objectType = layer.Properties.findEnum<btObjectType>(LayerPropertyKey.object_type, btObjectType.none);
        this.ParseExtraLayerDataFields(layer, layerData);
        layerData.thingDatas = new List<btThingData>();

        bool isStair = layerData.objectType == btObjectType.stair;
        int maxZ = 0;
        bool foundMaxZ = false;
        if (isStair)
        {
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

                // int x = j % map.Width;
                int z = j / map.Width;

                if (!foundMaxZ || z > maxZ)
                {
                    foundMaxZ = true;
                    maxZ = z;
                }
            }
        }

        if (foundMaxZ)
        {
            Debug.Log("maxZ = " +maxZ);
        }

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
            int y = 0;
            int z = j / map.Width;

            if (isStair && foundMaxZ)
            {
                y = maxZ - z;
                z = maxZ;
            }

            btThingData thingData = new btThingData();
            thingData.id = this.getNextObjectId();
            thingData.tileset = ts.source;
            thingData.tileId = dataId - ts.firstgid;
            thingData.position.x = this.coordConverter.ConvertCoordX(x);
            thingData.position.y = this.coordConverter.ConvertCoordY(y);
            thingData.position.z = this.coordConverter.ConvertCoordZ(z);
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

        this.coordConverter.pixelsPerTileX = map.TileWidth;
        this.coordConverter.pixelsPerTileYZ = map.TileHeight;
        this.coordConverter.originTile = new Vector3Int(
            map.Properties.findInt(TilemapPropertyKey.x_origin, -1),
            map.Properties.findInt(TilemapPropertyKey.y_origin, -1),
            map.Properties.findInt(TilemapPropertyKey.z_origin, -1));

        if (this.coordConverter.originTile.x == -1 ||
            this.coordConverter.originTile.y == -1 ||
            this.coordConverter.originTile.z == -1)
        {
            throw new Exception("x_origin || y_origin || z_origin not defined");
        }

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

            btTileLayerData layerData = this.ParseLayer(map, layer);
            mapData.layerDatas.Add(layerData);
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(mapData, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

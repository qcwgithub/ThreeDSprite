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
    // .tsx -> .json
    // 变成与 tiled 编辑器无关的格式
    void ImportTileset(string fileName)
    {
        var tsxPath = tiledDir + "/" + fileName;
        var tileset = new TiledTileset(tsxPath);
        // Debug.Log("Load success");

        btThingShape shapeInParent;
        bool hasShapeInParent = tileset.Properties.findEnum<btThingShape>(TilesetPropertyKey.child_shape, out shapeInParent);

        btObjectType objectTypeInParent;
        bool hasObjectTypeInParent = tileset.Properties.findEnum<btObjectType>(TilesetPropertyKey.child_object_type, out objectTypeInParent);

        // 
        btTilesetConfig tilesetConfig = new btTilesetConfig();
        tilesetConfig.tiles = new Dictionary<int, btThingConfig>();

        foreach (TiledTile tile in tileset.Tiles)
        {
            btThingConfig thingConfig = new btThingConfig();
            tilesetConfig.tiles.Add(tile.id, thingConfig);

            // spriteName
            TiledTileImage image = tile.image;
            thingConfig.spriteName = Path.GetFileNameWithoutExtension(image.source);

            // shape
            thingConfig.shape = btThingShape.cube;
            if (!tile.properties.findEnum<btThingShape>(TilePropertyKey.shape, out thingConfig.shape))
            {
                if (!hasShapeInParent)
                {
                    throw new Exception("shape not defined");
                }
                else
                {
                    thingConfig.shape = shapeInParent;
                }
            }

            // size
            switch (thingConfig.shape)
            {
                case btThingShape.cube:
                    {
                        int cube_y_height = tile.properties.findInt(TilePropertyKey.cube_y_height, -1);
                        if (cube_y_height == -1)
                        {
                            throw new Exception(TilePropertyKey.cube_y_height + " not defined");
                        }

                        thingConfig.size = new LVector3 { x = image.width, y = cube_y_height, z = image.height - cube_y_height };
                    }
                    break;

                case btThingShape.xy:
                    {
                        thingConfig.size = new LVector3 { x = image.width, y = image.height, z = 0 };
                    }
                    break;

                case btThingShape.xz:
                    {
                        thingConfig.size = new LVector3 { x = image.width, y = 0, z = image.height };
                    }
                    break;

                default:
                    throw new Exception("unknow shape");
                    // break;
            }

            // objectType
            thingConfig.objectType = btObjectType.none;
            btObjectType objectType;
            if (tile.properties.findEnum<btObjectType>(TilePropertyKey.object_type, out objectType))
            {
                thingConfig.objectType = objectType;
            }
            else if (hasObjectTypeInParent)
            {
                thingConfig.objectType = objectTypeInParent;
            }
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(tilesetConfig, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

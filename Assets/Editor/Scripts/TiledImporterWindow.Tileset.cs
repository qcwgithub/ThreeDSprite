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

        btShape shapeInParent;
        bool hasShapeInParent = tileset.Properties.findEnum<btShape>(TilesetPropertyKey.tile_shape, out shapeInParent);

        btObjectType objectTypeInParent;
        bool hasObjectTypeInParent = tileset.Properties.findEnum<btObjectType>(TilesetPropertyKey.tile_object_type, out objectTypeInParent);

        // 
        btTilesetConfig tilesetConfig = new btTilesetConfig();
        tilesetConfig.tiles = new Dictionary<int, btTileConfig>();

        foreach (TiledTile tile in tileset.Tiles)
        {
            btTileConfig tileConfig = new btTileConfig();
            tilesetConfig.tiles.Add(tile.id, tileConfig);

            // spriteName
            TiledTileImage image = tile.image;
            tileConfig.spriteName = Path.GetFileNameWithoutExtension(image.source);

            // shape
            tileConfig.shape = btShape.cube;
            if (!tile.properties.findEnum<btShape>(TilePropertyKey.shape, out tileConfig.shape))
            {
                if (!hasShapeInParent)
                {
                    throw new Exception("shape not defined");
                }
                else
                {
                    tileConfig.shape = shapeInParent;
                }
            }

            FVector3 pixelSize = new FVector3 { x = 0f, y = 0f, z = 0f };
            // size
            switch (tileConfig.shape)
            {
                case btShape.cube:
                    {
                        int cube_y_height = tile.properties.findInt(TilePropertyKey.cube_y_height, -1);
                        if (cube_y_height == -1)
                        {
                            throw new Exception(TilePropertyKey.cube_y_height + " not defined");
                        }

                        pixelSize = new FVector3 { x = image.width, y = cube_y_height, z = image.height - cube_y_height };
                    }
                    break;

                case btShape.xy:
                    {
                        pixelSize = new FVector3 { x = image.width, y = image.height, z = 0 };
                    }
                    break;

                case btShape.xz:
                    {
                        pixelSize = new FVector3 { x = image.width, y = 0, z = image.height };
                    }
                    break;

                default:
                    throw new Exception("unknow shape");
                    // break;
            }

            tileConfig.size.x = pixelSize.x / btConstants.pixels_per_unit;
            tileConfig.size.y = pixelSize.y / btConstants.pixels_per_unit * btConstants.sqrt2;
            tileConfig.size.z = pixelSize.z / btConstants.pixels_per_unit * btConstants.sqrt2;

            // objectType
            tileConfig.objectType = btObjectType.none;
            btObjectType objectType;
            if (tile.properties.findEnum<btObjectType>(TilePropertyKey.object_type, out objectType))
            {
                tileConfig.objectType = objectType;
            }
            else if (hasObjectTypeInParent)
            {
                tileConfig.objectType = objectTypeInParent;
            }
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(tilesetConfig, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

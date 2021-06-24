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
    void ImportTileset(string fileName)
    {
        var tsxPath = tiledDir + "/" + fileName;
        var tileset = new TiledTileset(tsxPath);
        // Debug.Log("Load success");

        btThingShape shapeInParent;
        bool hasShapeInParent = tileset.Properties.findEnum<btThingShape>("shape", out shapeInParent);

        // 
        btTilesetConfig tilesetConfig = new btTilesetConfig();
        foreach (TiledTile tile in tileset.Tiles)
        {
            TiledTileImage image = tile.image;
            btThingShape shape = btThingShape.cube;
            if (!tile.properties.findEnum<btThingShape>("shape", out shape))
            {
                if (!hasShapeInParent)
                {
                    throw new Exception("shape not defined");
                }
                else
                {
                    shape = shapeInParent;
                }
            }

            string sourceWithoutExt = Path.GetFileNameWithoutExtension(image.source);

            switch (shape)
            {
                case btThingShape.cube:
                    {
                        int y_height = tile.properties.findInt("y_height", -1);
                        if (y_height == -1)
                        {
                            throw new Exception("y_height not defined");
                        }

                        var thing = new btThingConfigCube { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = y_height, z = image.height - y_height };
                        if (tilesetConfig.cubes == null)
                        {
                            tilesetConfig.cubes = new Dictionary<int, btThingConfigCube>();
                        }
                        tilesetConfig.cubes.Add(tile.id, thing);
                    }
                    break;

                case btThingShape.xy:
                    {
                        var thing = new btThingConfigXY { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = image.height, z = 0 };
                        if (tilesetConfig.xys == null)
                        {
                            tilesetConfig.xys = new Dictionary<int, btThingConfigXY>();
                        }
                        tilesetConfig.xys.Add(tile.id, thing);
                    }
                    break;

                case btThingShape.xz:
                    {
                        var thing = new btThingConfigXZ { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = 0, z = image.height };
                        if (tilesetConfig.xzs == null)
                        {
                            tilesetConfig.xzs = new Dictionary<int, btThingConfigXZ>();
                        }
                        tilesetConfig.xzs.Add(tile.id, thing);
                    }
                    break;

                default:
                    throw new Exception("unknow shape");
                    // break;
            }
        }

        Directory.CreateDirectory(this.importedDir);
        var jsonPath = this.importedDir + "/" + fileName + ".json";

        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(tilesetConfig, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }
}

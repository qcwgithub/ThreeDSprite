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

public class TiledImporterWindow : EditorWindow
{
    [MenuItem("TDS/Tiled Importer Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TiledImporterWindow>();
    }

    private List<string> tiledFiles = new List<string>();
    const string tarentDirectory = "Assets/Egzd";
    string tiledDirectory = tarentDirectory + "/Tiled";
    string importedDirectory = tarentDirectory + "/Imported";

    void RefreshFiles()
    {
        this.tiledFiles.Clear();
        string[] files = Directory.GetFiles(tiledDirectory, "*.tmx", SearchOption.TopDirectoryOnly);
        files = files.Concat(Directory.GetFiles(tiledDirectory, "*.tsx", SearchOption.TopDirectoryOnly)).ToArray();

        // string currentFullPath = Path.GetFullPath("Assets");
        foreach (var file in files)
        {
            //string fullPath = Path.GetFullPath(file).Replace('\\', '/');

            this.tiledFiles.Add(Path.GetFileName(file));
        }
    }

    void LoadTilemap(string fileName)
    {
        var filePath = tiledDirectory + "/" + fileName;
        var map = new TiledMap(filePath);
        Debug.Log(string.Format("map size: {0} x {1}", map.Width, map.Height));
        Debug.Log(string.Format("map tile size: {0} x {1}", map.TileWidth, map.TileHeight));

        var config = new btMapConfig();
        config.x_origin = map.Properties.findInt("x_origin", -1);
        config.y_origin = map.Properties.findInt("y_origin", -1);

        List<TextAsset> tilesetAssets = new List<TextAsset>();

        for (int i = 0; i < map.Tilesets.Length; i++)
        {
            TiledMapTileset tileset = map.Tilesets[i];
            string tilesetPath = this.importedDirectory + "/" + tileset.source + ".json";
            TextAsset tilesetAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tilesetPath);
            if (tilesetAsset == null)
            {
                Debug.LogError(tilesetPath + " not found");
                break;
            }
            Debug.Log("loaded tileset: " + tilesetPath);
            tilesetAssets.Add(tilesetAsset);
        }

        config.tileset_gids = new List<btMapConfig.Tileset_FirstGid>();
        for (int i = 0; i < map.Tilesets.Length; i++)
        {
            TiledMapTileset tileset = map.Tilesets[i];
            config.tileset_gids.Add(new btMapConfig.Tileset_FirstGid { source = tileset.source, firstgid = tileset.firstgid });
        }

        for (int i = 0; i < map.Layers.Length; i++)
        {
            TiledLayer layer = map.Layers[i];
            if (layer.data == null)
                continue;

            // Debug.Log("layer.data.length = " + layer.data.Length);
            for (int j = 0; j < layer.data.Length; j++)
            {
                int data = layer.data[j];
                if (data == 0) continue;

                int x = j % map.Width;
                int y = j / map.Width;

                int pixelx = (x - config.x_origin) * map.TileWidth;
                int pixely = (y - config.y_origin) * map.TileHeight;

            }
        }
        // Debug.Log("Load success");
    }

    void LoadTileset(string fileName)
    {
        var tsxPath = tiledDirectory + "/" + fileName;
        var tileset = new TiledTileset(tsxPath);
        // Debug.Log("Load success");

        btThingShape shapeInParent;
        bool hasShapeInParent = tileset.Properties.findEnum<btThingShape>("shape", out shapeInParent);

        // 
        var config = new btTilesetConfig();
        foreach (TiledTile tile in tileset.Tiles)
        {
            TiledTileImage image = tile.image;
            btThingShape shape = btThingShape.cube;
            if (!tile.properties.findEnum<btThingShape>("shape", out shape))
            {
                if (!hasShapeInParent)
                    throw new Exception("shape not defined");
                else
                    shape = shapeInParent;
            }

            string sourceWithoutExt = Path.GetFileNameWithoutExtension(image.source);

            switch (shape)
            {
                case btThingShape.cube:
                    {
                        int y_height = tile.properties.findInt("y_height", -1);
                        if (y_height == -1) throw new Exception("y_height not defined");
                        
                        var thing = new btThingConfigCube { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = y_height, z = image.height - y_height };
                        if (config.cubes == null) config.cubes = new Dictionary<int, btThingConfigCube>();
                        config.cubes.Add(tile.id, thing);
                    }
                    break;
                case btThingShape.xy:
                    {
                        var thing = new btThingConfigXY { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = image.height, z = 0 };
                        if (config.xys == null) config.xys = new Dictionary<int, btThingConfigXY>();
                        config.xys.Add(tile.id, thing);
                    }
                    break;
                case btThingShape.xz:
                    {
                        var thing = new btThingConfigXZ { spriteName = sourceWithoutExt };
                        thing.size = new LVector3 { x = image.width, y = 0, z = image.height };
                        if (config.xzs == null) config.xzs = new Dictionary<int, btThingConfigXZ>();
                        config.xzs.Add(tile.id, thing);
                    }
                    break;
                default:
                    throw new Exception("unknow shape");
                    // break;
            }
        }

        Directory.CreateDirectory(this.importedDirectory);
        var jsonPath = this.importedDirectory + "/" + fileName + ".json";
        
        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        AssetDatabase.Refresh();
        Debug.Log("success, file: " + jsonPath);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Tiled Files Directory: ");
        GUILayout.Label(tarentDirectory);

        if (GUILayout.Button("Refresh Tiled Files"))
        {
            this.RefreshFiles();
        }

        bool all = false;
        // if (this.tiledFiles.Count > 1)
        // {
        //     all = GUILayout.Button("all");
        // }

        foreach (var file in this.tiledFiles)
        {
            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(file);
            if (all || GUILayout.Button(file))
            {
                // Debug.Log("to do: load " + file);
                if (file.EndsWith(".tmx"))
                {
                    this.LoadTilemap(file);
                }
                else
                {
                    this.LoadTileset(file);
                }
            }
            // EditorGUILayout.EndHorizontal();
        }
    }
}

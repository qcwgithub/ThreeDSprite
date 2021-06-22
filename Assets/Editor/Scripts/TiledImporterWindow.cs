using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TiledCS;

public class TiledImporterWindow : EditorWindow
{
    [MenuItem("TDS/Tiled Importer Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TiledImporterWindow>();
    }

    private List<string> tiledFiles = new List<string>();
    const string tarentDirectory = "./Assets/Egzd";
    string tiledDirectory = tarentDirectory + "/Tiled";

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
        Debug.Log("Load success");
    }

    void LoadTileset(string fileName)
    {
        var tsxPath = tiledDirectory + "/" + fileName;
        var tileset = new TiledTileset(tsxPath);
        // Debug.Log("Load success");

        // 
        // var jsonPath = 
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Tiled Files Directory: ");
        tiledDirectory = GUILayout.TextField(tiledDirectory);

        if (GUILayout.Button("Refresh Tiled Files"))
        {
            this.RefreshFiles();
        }

        bool all = false;
        if (this.tiledFiles.Count > 1)
        {
            all = GUILayout.Button("all");
        }

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

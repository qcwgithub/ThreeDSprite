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
    private string tiledFilesDirectory = "./TiledProject";

    void RefreshFiles()
    {
        this.tiledFiles.Clear();
        string[] files = Directory.GetFiles(this.tiledFilesDirectory, "*.tmx", SearchOption.TopDirectoryOnly);
        files = files.Concat(Directory.GetFiles(this.tiledFilesDirectory, "*.tsx", SearchOption.TopDirectoryOnly)).ToArray();

        // string currentFullPath = Path.GetFullPath("Assets");
        foreach (var file in files)
        {
            //string fullPath = Path.GetFullPath(file).Replace('\\', '/');

            this.tiledFiles.Add(Path.GetFileName(file));
        }
    }

    void LoadTilemap(string fileName)
    {
        var filePath = this.tiledFilesDirectory + "/" + fileName;
        var map = new TiledMap(filePath);
        Debug.Log("Load success");
    }

    void LoadTileset(string fileName)
    {
        var filePath = this.tiledFilesDirectory + "/" + fileName;
        var map = new TiledTileset(filePath);
        Debug.Log("Load success");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Tiled Files Directory: ");
        this.tiledFilesDirectory = GUILayout.TextField(this.tiledFilesDirectory);

        if (GUILayout.Button("Refresh Tiled Files"))
        {
            this.RefreshFiles();
        }

        foreach (var file in this.tiledFiles)
        {
            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(file);
            if (GUILayout.Button(file))
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

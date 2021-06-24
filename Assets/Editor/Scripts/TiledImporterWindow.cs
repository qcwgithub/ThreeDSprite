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

public partial class TiledImporterWindow : EditorWindow
{
    [MenuItem("TDS/Tiled Importer Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TiledImporterWindow>();
    }

    const string parentDir = "Assets/Egzd";
    string atlasDir = parentDir + "/Atlas";
    string tiledDir = parentDir + "/Tiled";
    string importedDir = parentDir + "/Imported";

    List<string> tiledFiles = new List<string>();
    List<string> importedFiles = new List<string>();
    void RefreshFiles()
    {
        this.tiledFiles.Clear();
        string[] files = Directory.GetFiles(this.tiledDir, "*.tmx", SearchOption.TopDirectoryOnly);
        files = files.Concat(Directory.GetFiles(this.tiledDir, "*.tsx", SearchOption.TopDirectoryOnly)).ToArray();
        
        foreach (var file in files)
        {
            this.tiledFiles.Add(Path.GetFileName(file));
        }

        //-----------------------------------
        this.importedFiles.Clear();
        files = Directory.GetFiles(this.importedDir, "*.tmx.json", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            this.importedFiles.Add(Path.GetFileName(file));
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Tiled Files Directory: ");
        GUILayout.Label(parentDir);

        if (GUILayout.Button("Refresh Files"))
        {
            this.RefreshFiles();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Import to Json:");

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
                    this.ImportTilemap(file);
                }
                else
                {
                    this.ImportTileset(file);
                }
            }
            // EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Import to Prefab:");
        foreach (var file in this.importedFiles)
        {
            if (GUILayout.Button(file))
            {
                this.ImportPrefab(file);
            }
        }
    }
}

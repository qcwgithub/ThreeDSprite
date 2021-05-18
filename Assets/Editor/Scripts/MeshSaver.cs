using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshSaverEditor
{
    [MenuItem("Assets/CreateCube")]
    public static void CreateCube()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            // front
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),

            // back
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
        };
        mesh.triangles = new int[]
        {
            // front
            0,2,1,
            1,2,3,
            // back
            //5,7,4,
            //7,6,4,
            // top
            3,2,6,
            3,6,7,
            // bottom
            //0,1,4,
            //1,5,4,
            // left
            //2,0,4,
            //2,4,6,
            // right
            //1,3,7,
            //1,7,5,
        };
        mesh.uv = new Vector2[]
        {
            new Vector2(0f,0f),
            new Vector2(1f,0f),
            new Vector2(0f,1-0.4458f),
            new Vector2(1f,1-0.4458f),


            new Vector2(1f,1-0.4458f),
            new Vector2(0f,1-0.4458f),

            new Vector2(0f,1f),
            new Vector2(1f,1f),
        };
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", "MyCube", "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);
        MeshUtility.Optimize(mesh);

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
    public static void SaveMeshInPlace(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, false, true);
    }

    [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
    public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, true, true);
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

}
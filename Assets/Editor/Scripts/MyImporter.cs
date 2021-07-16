using UnityEngine;
using UnityEditor;

using System.IO;


namespace Assets.Editor
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, new string[] { "cube", "cubes" })]
    public class MyImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public int pixelsPerUnit = 100;
        public MyShape shape = MyShape.Cube;

        private void ImportOne(UnityEditor.AssetImporters.AssetImportContext ctx, string texturePath)
        {
            string name = Path.GetFileNameWithoutExtension(texturePath);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                Debug.LogWarning("texture is null, " + texturePath);
                return;
            }
            float texWidth = texture.width;
            float texHeight = texture.height;

            // new material
            var material = new Material(Shader.Find("My/Unlit/Transparent"));
            material.mainTexture = texture;
            material.name = name;
            ctx.AddObjectToAsset("material_" + name, material);

            float belowPercent = 0f;
            if (this.shape == MyShape.Cube)
            {
                int index = name.LastIndexOf('_');
                if (index < 0)
                {
                    Debug.LogWarning("index < 0, " + texturePath);
                    return;
                }

                if (!float.TryParse(name.Substring(index + 1), out belowPercent))
                {
                    Debug.LogWarning("!float.TryParse, " + texturePath);
                    return;
                }
            }


            // new mesh
            Mesh mesh = null;
            switch (shape)
            {
                case MyShape.Cube:
                    mesh = MyImporterUtils.CreateMesh_Cube(texWidth, texHeight, pixelsPerUnit, belowPercent);
                    break;

                case MyShape.Quad_XZ:
                    mesh = MyImporterUtils.CreateMesh_QuadXZ(texWidth, texHeight, pixelsPerUnit);
                    break;

                case MyShape.Quad_XY:
                    mesh = MyImporterUtils.CreateMesh_QuadXY(texWidth, texHeight, pixelsPerUnit);
                    break;
            }
            mesh.name = name;
            ctx.AddObjectToAsset("mesh_" + name, mesh);

            // new gameobject

            GameObject go = new GameObject(name);
            go.name = name;
            ctx.AddObjectToAsset("go_" + name, go);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = material;

            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            switch (this.shape)
            {
                case MyShape.Cube:
                    MyImporterUtils.SetupBoxCollider_Cube(collider, texWidth, texHeight, this.pixelsPerUnit, belowPercent);
                    break;
                case MyShape.Quad_XZ:
                    MyImporterUtils.SetupBoxCollider_QuadXZ(collider, texWidth, texHeight, this.pixelsPerUnit);
                    break;
                case MyShape.Quad_XY:
                    MyImporterUtils.SetupCollider_QuadXY(collider, texWidth, texHeight, this.pixelsPerUnit);
                    break;
            }
        }


        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            string assetPath = ctx.assetPath;
            if (assetPath.EndsWith(".cube"))
            {
                // load texture
                string texturePath = ctx.assetPath.Replace(".cube", ".png");
                this.ImportOne(ctx, texturePath);
            }
            else
            {
                // 添加一个假的
                GameObject main = new GameObject("main");
                ctx.AddObjectToAsset("main obj", main);

                string dir = Path.GetDirectoryName(assetPath);
                string[] files = Directory.GetFiles(dir, "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    this.ImportOne(ctx, file);
                }
            }
        }
    }
}

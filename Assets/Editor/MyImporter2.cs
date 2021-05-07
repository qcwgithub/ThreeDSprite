using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace Assets.Editor
{
    public enum MyShape
    {
        Cube,
        Quad_XZ,
        Quad_XY,
    }

    public enum MyQuadDir
    {
        XZ,
        XY,
    }

    public enum MyColliderType
    {
        None,
        Box,
        Capsule,
    }

    // cube
    // quad xz horizontal
    // quad xy vertical
    [UnityEditor.AssetImporters.ScriptedImporter(1, new string[] { "cube", "cubes" })]
    public class MyImporter2 : UnityEditor.AssetImporters.ScriptedImporter
    {
        public MyShape shape = MyShape.Cube;
        public MyQuadDir quadDir = MyQuadDir.XZ;
        public MyColliderType addCollider = MyColliderType.Box;

        const float SQRT2 = 1.4142135623730951f;

        static void SetupCubeBoxCollider(BoxCollider collider, 
            float texWidth, float texHeight, float pixelsPerUnit, float belowPercent)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = texHeight * belowPercent / pixelsPerUnit;
            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, yLength / 2, zLength / 2);
        }

        static void SetupQuadXZBoxCollider(BoxCollider collider,
            float texWidth, float texHeight, float pixelsPerUnit)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = 0f;// texHeight * belowPercent / pixelsPerUnit;
            float zLength = texHeight * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, yLength / 2, -zLength / 2);
        }

        static void SetupQuadXYBoxCollider(BoxCollider collider,
            float texWidth, float texHeight, float pixelsPerUnit)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = texHeight / pixelsPerUnit;
            float zLength = 0f;// texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, yLength / 2, zLength / 2);
        }

        private void ImportOne(UnityEditor.AssetImporters.AssetImportContext ctx, string spritePath)
        {
            string name = Path.GetFileNameWithoutExtension(spritePath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning("sprite is null, " + spritePath);
                return;
            }
            float texWidth = sprite.rect.width;
            float texHeight = sprite.rect.height;

            GameObject go = new GameObject(name);
            ctx.AddObjectToAsset("go_" + name, go);
            //if (ctx.mainObject == null)
            //{
            //    ctx.SetMainObject(go);
            //}
            Transform trans = go.transform;
            trans.eulerAngles = new Vector3(45f, 0f, 0f);

            // sprites
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.spriteSortPoint = SpriteSortPoint.Pivot;
            renderer.sprite = sprite;

            GameObject goCollider = new GameObject("collider");
            BoxCollider collider = goCollider.AddComponent<BoxCollider>();
            switch (this.shape)
            {
                case MyShape.Cube:
                    {
                        int index = name.LastIndexOf('_');
                        if (index < 0)
                        {
                            Debug.LogWarning("index < 0, " + spritePath);
                            return;
                        }
                        float belowPercent = 0f;
                        if (!float.TryParse(name.Substring(index + 1), out belowPercent))
                        {
                            Debug.LogWarning("!float.TryParse, " + spritePath);
                            return;
                        }
                        SetupCubeBoxCollider(collider, texWidth, texHeight, sprite.pixelsPerUnit, belowPercent);
                    }
                    break;
                case MyShape.Quad_XZ:
                    SetupQuadXZBoxCollider(collider, texWidth, texHeight, sprite.pixelsPerUnit);
                    break;
                case MyShape.Quad_XY:
                    SetupQuadXYBoxCollider(collider, texWidth, texHeight, sprite.pixelsPerUnit);
                    break;
            }

            Transform transCollider = goCollider.transform;
            transCollider.parent = trans;
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

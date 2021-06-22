using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

using System.IO;
using UnityEditor.Experimental.AssetImporters;

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
    //[UnityEditor.AssetImporters.ScriptedImporter(1, new string[] { "cube", "cubes" })]
    public class MyImporter2 : ScriptedImporter
    {
        public MyShape shape = MyShape.Cube;
        public MyQuadDir quadDir = MyQuadDir.XZ;      

        private void ImportOne(AssetImportContext ctx, string spritePath)
        {
            string name = Path.GetFileNameWithoutExtension(spritePath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning("sprite is null, " + spritePath);
                return;
            }

            float spriteWidth = sprite.rect.width;
            float spriteHeight = sprite.rect.height;

            GameObject go = new GameObject(name);
            ctx.AddObjectToAsset("go_" + name, go);

            Transform trans = go.transform;
            trans.eulerAngles = new Vector3(45f, 0f, 0f);

            // sprites
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.spriteSortPoint = SpriteSortPoint.Pivot;
            renderer.sprite = sprite;

            GameObject goCollider = new GameObject("collider");
            BoxCollider collider = goCollider.AddComponent<BoxCollider>();
            collider.isTrigger = true;
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
                        MyImporterUtils.SetupBoxCollider_Cube(collider, spriteWidth, spriteHeight, sprite.pixelsPerUnit, belowPercent);
                    }
                    break;
                case MyShape.Quad_XZ:
                    MyImporterUtils.SetupBoxCollider_QuadXZ(collider, spriteWidth, spriteHeight, sprite.pixelsPerUnit);
                    break;
                case MyShape.Quad_XY:
                    MyImporterUtils.SetupCollider_QuadXY(collider, spriteWidth, spriteHeight, sprite.pixelsPerUnit);
                    break;
            }

            Transform transCollider = goCollider.transform;
            transCollider.parent = trans;
        }

        public override void OnImportAsset(AssetImportContext ctx)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

namespace Assets.Editor
{
    public enum MyCubeImporterType
    {
        FullCube,
        Top_Front,
    }


    [ScriptedImporter(1, "cube")]
    public class MyImporter : ScriptedImporter
    {
        public MyCubeImporterType type = MyCubeImporterType.FullCube;
        [Range(0f, 1f)]
        public float belowPercent = 0.5f;
        public int pixelsPerUnit = 100;
        public Vector3 pivot = new Vector3(0.5f, 0f, 0.5f);
        public bool needOptimize = true;
        const float SQRT2 = 1.4142135623730951f;

        Mesh CreateFullCube(float xLength, float yLength, float zLength)
        {
            Mesh mesh = new Mesh();
            var vertices = new Vector3[]
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
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].x *= xLength;
                vertices[i].y *= yLength;
                vertices[i].z *= zLength;
            }

            mesh.vertices = vertices;
            
            mesh.triangles = new int[]
            {
                // front
                0,2,1,
                1,2,3,
                // back
                5,7,4,
                7,6,4,
                // top
                3,2,6,
                3,6,7,
                // bottom
                0,1,4,
                1,5,4,
                // left
                2,0,4,
                2,4,6,
                // right
                1,3,7,
                1,7,5,
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, this.belowPercent),
                new Vector2(1f, this.belowPercent),


                new Vector2(1f, this.belowPercent),
                new Vector2(0f, this.belowPercent),

                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };

            if (this.needOptimize)
            {
                MeshUtility.Optimize(mesh);
            }
            return mesh;
        }

        Mesh CreateTopFront(float xLength, float yLength, float zLength)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),

                // back
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
            };
            mesh.triangles = new int[]
            {
                // front
                0,2,1,
                1,2,3,

                // top
                3,2,4,
                3,4,5,
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, this.belowPercent),
                new Vector2(1f, this.belowPercent),

                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };

            if (this.needOptimize)
            {
                MeshUtility.Optimize(mesh);
            }
            return mesh;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // load texture
            string texturePath = ctx.assetPath.Replace(".cube", ".png");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            // new material
            var material = new Material(Shader.Find("Unlit/Transparent"));
            material.mainTexture = texture;
            ctx.AddObjectToAsset("my material", material);

            // new mesh

            float texWidth = texture.width;
            float texHeight = texture.height;

            float xLength = texWidth / this.pixelsPerUnit;
            float yLength = texHeight * this.belowPercent * SQRT2 / this.pixelsPerUnit;
            float zLength = texHeight * (1f - this.belowPercent) * SQRT2 / this.pixelsPerUnit;

            Mesh mesh = null;
            if (this.type == MyCubeImporterType.FullCube)
            {
                mesh = this.CreateFullCube(xLength, yLength, zLength);
            }
            else
            {
                mesh = this.CreateTopFront(xLength, yLength, zLength);
            }
            ctx.AddObjectToAsset("my mesh", mesh);

            // new gameobject
            string name = Path.GetFileNameWithoutExtension(ctx.assetPath);

            GameObject go = new GameObject(name);
            ctx.AddObjectToAsset("my go", go);
            ctx.SetMainObject(go);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = material;
        }
    }
}

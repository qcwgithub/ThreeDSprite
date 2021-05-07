//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEditor;

//using System.IO;

//namespace Assets.Editor
//{

//    //[UnityEditor.AssetImporters.ScriptedImporter(1, new string[] { "cube", "cubes" })]
//    public class MyImporter : UnityEditor.AssetImporters.ScriptedImporter
//    {
//        public int pixelsPerUnit = 100;
//        public MyShape shape = MyShape.Cube;
//        [Range(0f, 1f)]
//        public float belowPercent = 0.5f;
//        public MyQuadDir quadDir = MyQuadDir.XZ;
//        public Vector3 pivot = new Vector3(0.5f, 0f, 0f);
//        public MyColliderType addCollider = MyColliderType.Box;

//        const float SQRT2 = 1.4142135623730951f;

//        static void AttachSpriteRenderer(GameObject go)
//        {
//            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
//            renderer.spriteSortPoint = SpriteSortPoint.Pivot;
//        }

//        static Mesh CreateSpriteMesh(float texWidth, float texHeight, int pixelsPerUnit, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            float yLength = texHeight / pixelsPerUnit;
//            float zLength = 0f;

//            Mesh mesh = new Mesh();
//            var vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, 0f, 0f),
//                new Vector3(0.5f, 0f, 0f),
//                new Vector3(-0.5f, 1f, 0f),
//                new Vector3(0.5f, 1f, 0f),
//            };
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].x += (0.5f - pivot.x);
//                vertices[i].x *= xLength;

//                vertices[i].y += (0.5f - pivot.y);
//                vertices[i].y *= yLength;

//                vertices[i].z += (0.5f - pivot.z);
//                vertices[i].z *= zLength;
//            }
//            mesh.vertices = vertices;
//            mesh.triangles = new int[]
//            {
//                // front
//                0,2,1,
//                1,2,3,
//            };
//            mesh.uv = new Vector2[]
//            {
//                new Vector2(0f, 0f),
//                new Vector2(1f, 0f),
//                new Vector2(0f, 1f),
//                new Vector2(1f, 1f),
//            };

//            //if (this.needOptimize)
//            //{
//            //    MeshUtility.Optimize(mesh);
//            //}
//            return mesh;
//        }

//        static void SetupBoxCollider(BoxCollider collider, 
//            float texWidth, float texHeight, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
//            float yLength = texHeight * belowPercent / pixelsPerUnit;
//            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

//            collider.size = new Vector3(xLength, yLength, zLength);
//            collider.center = new Vector3(0f, yLength/2, zLength/2);


//            //var vertices = new Vector3[]
//            //{
//            //    // front
//            //    new Vector3(-0.5f, -0.5f, -0.5f),
//            //    new Vector3(0.5f, -0.5f, -0.5f),
//            //    new Vector3(-0.5f, 0.5f, -0.5f),
//            //    new Vector3(0.5f, 0.5f, -0.5f),

//            //    // back
//            //    new Vector3(-0.5f, -0.5f, 0.5f),
//            //    new Vector3(0.5f, -0.5f, 0.5f),
//            //    new Vector3(-0.5f, 0.5f, 0.5f),
//            //    new Vector3(0.5f, 0.5f, 0.5f),
//            //};
//            //for (int i = 0; i < vertices.Length; i++)
//            //{
//            //    vertices[i].x += (0.5f - pivot.x);
//            //    vertices[i].x *= xLength;

//            //    vertices[i].y += (0.5f - pivot.y);
//            //    vertices[i].y *= yLength;

//            //    vertices[i].z += (0.5f - pivot.z);
//            //    vertices[i].z *= zLength;
//            //}

//            //collider.center = (vertices[0] + vertices[7]) / 2;

//            //Vector3 center = new Vector3(0f, 0f, 0f);
//            //center.x += (0.5f - pivot.x);
//            //center.y += (0.5f - pivot.y);
//            //center.z += (0.5f - pivot.z);
//            //collider.center = center;
//        }

//        static Mesh CreateCube(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
//            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

//            //Debug.Log(string.Format("length:{0}*{1}*{2}", xLength, yLength, zLength));
//            Mesh mesh = new Mesh();
//            var vertices = new Vector3[]
//            {
//                // front
//                new Vector3(-0.5f, -0.5f, -0.5f),
//                new Vector3(0.5f, -0.5f, -0.5f),
//                new Vector3(-0.5f, 0.5f, -0.5f),
//                new Vector3(0.5f, 0.5f, -0.5f),

//                // back
//                new Vector3(-0.5f, -0.5f, 0.5f),
//                new Vector3(0.5f, -0.5f, 0.5f),
//                new Vector3(-0.5f, 0.5f, 0.5f),
//                new Vector3(0.5f, 0.5f, 0.5f),
//            };
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].x += (0.5f - pivot.x);
//                vertices[i].x *= xLength;

//                vertices[i].y += (0.5f - pivot.y);
//                vertices[i].y *= yLength;

//                vertices[i].z += (0.5f - pivot.z);
//                vertices[i].z *= zLength;
//            }

//            mesh.vertices = vertices;
            
//            mesh.triangles = new int[]
//            {
//                // front
//                0,2,1,
//                1,2,3,
//                // back
//                5,7,4,
//                7,6,4,
//                // top
//                3,2,6,
//                3,6,7,
//                // bottom
//                0,1,4,
//                1,5,4,
//                // left
//                2,0,4,
//                2,4,6,
//                // right
//                1,3,7,
//                1,7,5,
//            };
//            mesh.uv = new Vector2[]
//            {
//                new Vector2(0f, 0f),
//                new Vector2(1f, 0f),
//                new Vector2(0f, belowPercent),
//                new Vector2(1f, belowPercent),


//                new Vector2(1f, belowPercent),
//                new Vector2(0f, belowPercent),

//                new Vector2(0f, 1f),
//                new Vector2(1f, 1f),
//            };

//            //if (this.needOptimize)
//            //{
//            //    MeshUtility.Optimize(mesh);
//            //}
//            return mesh;
//        }

//        static Mesh CreateTopFront(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
//            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

//            Mesh mesh = new Mesh();
//            var vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, -0.5f, -0.5f),
//                new Vector3(0.5f, -0.5f, -0.5f),
//                new Vector3(-0.5f, 0.5f, -0.5f),
//                new Vector3(0.5f, 0.5f, -0.5f),

//                // back
//                new Vector3(-0.5f, 0.5f, 0.5f),
//                new Vector3(0.5f, 0.5f, 0.5f),
//            };
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].x += (0.5f - pivot.x);
//                vertices[i].x *= xLength;

//                vertices[i].y += (0.5f - pivot.y);
//                vertices[i].y *= yLength;

//                vertices[i].z += (0.5f - pivot.z);
//                vertices[i].z *= zLength;
//            }
//            mesh.vertices = vertices;
//            mesh.triangles = new int[]
//            {
//                // front
//                0,2,1,
//                1,2,3,

//                // top
//                3,2,4,
//                3,4,5,
//            };
//            mesh.uv = new Vector2[]
//            {
//                new Vector2(0f, 0f),
//                new Vector2(1f, 0f),
//                new Vector2(0f, belowPercent),
//                new Vector2(1f, belowPercent),

//                new Vector2(0f, 1f),
//                new Vector2(1f, 1f),
//            };

//            //if (this.needOptimize)
//            //{
//            //    MeshUtility.Optimize(mesh);
//            //}
//            return mesh;
//        }
//        static Mesh CreateXZQuad(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            float zLength = texHeight * SQRT2 / pixelsPerUnit;

//            Mesh mesh = new Mesh();
//            var vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, 0f, -0.5f),
//                new Vector3(0.5f, 0f, -0.5f),
//                new Vector3(-0.5f, 0f, 0.5f),
//                new Vector3(0.5f, 0f, 0.5f),
//            };
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].x += (0.5f - pivot.x);
//                vertices[i].x *= xLength;

//                //vertices[i].y += (0.5f - pivot.y);
//                //vertices[i].y *= yLength;

//                vertices[i].z += (0.5f - pivot.z);
//                vertices[i].z *= zLength;
//            }
//            mesh.vertices = vertices;
//            mesh.triangles = new int[]
//            {
//                0,2,1,
//                1,2,3,
//            };
//            mesh.uv = new Vector2[]
//            {
//                new Vector2(0f, 0f),
//                new Vector2(1f, 0f),
//                new Vector2(0f, 1f),
//                new Vector2(1f, 1f),
//            };
//            return mesh;
//        }
//        static Mesh CreateXYQuad(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            float xLength = texWidth / pixelsPerUnit;
//            float yLength = texHeight * SQRT2 / pixelsPerUnit;

//            Mesh mesh = new Mesh();
//            var vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, -0.5f, 0f),
//                new Vector3(0.5f, -0.5f, 0f),
//                new Vector3(-0.5f, 0.5f, 0f),
//                new Vector3(0.5f, 0.5f, 0f),
//            };
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].x += (0.5f - pivot.x);
//                vertices[i].x *= xLength;

//                vertices[i].y += (0.5f - pivot.y);
//                vertices[i].y *= yLength;

//                //vertices[i].z += (0.5f - pivot.z);
//                //vertices[i].z *= zLength;
//            }
//            mesh.vertices = vertices;
//            mesh.triangles = new int[]
//            {
//                0,2,1,
//                1,2,3,
//            };
//            mesh.uv = new Vector2[]
//            {
//                new Vector2(0f, 0f),
//                new Vector2(1f, 0f),
//                new Vector2(0f, 1f),
//                new Vector2(1f, 1f),
//            };
//            return mesh;
//        }
//        static Mesh CreateMesh(float texWidth, float texHeight, MyShape shape, MyQuadDir quadDir, int pixelsPerUnit, float belowPercent, Vector3 pivot)
//        {
//            Mesh mesh = null;
//            switch (shape)
//            {
//                case MyShape.Cube:
//                    mesh = CreateCube(texWidth, texHeight, pixelsPerUnit, belowPercent, pivot);
//                    break;

//                case MyShape.Top_Front:
//                    mesh = CreateTopFront(texWidth, texHeight, pixelsPerUnit, belowPercent, pivot);
//                    break;

//                case MyShape.Quad:
//                    {
//                        if (quadDir == MyQuadDir.XZ)
//                            mesh = CreateXZQuad(texWidth, texHeight, pixelsPerUnit, belowPercent, pivot);
//                        else
//                            mesh = CreateXYQuad(texWidth, texHeight, pixelsPerUnit, belowPercent, pivot);
//                    }
//                    break;
//            }

//            return mesh;
//        }

//        private void ImportOne(UnityEditor.AssetImporters.AssetImportContext ctx, string texturePath)
//        {
//            string name = Path.GetFileNameWithoutExtension(texturePath);
//            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
//            if (texture == null)
//            {
//                return;
//            }
//            float texWidth = texture.width;
//            float texHeight = texture.height;

//            // new material
//            var material = new Material(Shader.Find("My/Unlit/Transparent"));
//            material.mainTexture = texture;
//            material.name = name;
//            ctx.AddObjectToAsset("material_" + name, material);

//            // new mesh
//            //Mesh mesh = CreateMesh(texWidth, texHeight, this.shape, this.quadDir, this.pixelsPerUnit, this.belowPercent, this.pivot);
//            Mesh mesh = CreateSpriteMesh(texWidth, texHeight, pixelsPerUnit, pivot);
//            mesh.name = name;
//            ctx.AddObjectToAsset("mesh_" + name, mesh);

//            // new gameobject

//            GameObject go = new GameObject(name);
//            go.name = name;
//            ctx.AddObjectToAsset("go_" + name, go);
//            if (ctx.mainObject==null)
//            {
//                ctx.SetMainObject(go);
//            }

//            MeshFilter filter = go.AddComponent<MeshFilter>();
//            filter.mesh = mesh;

//            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
//            renderer.material = material;

//            switch (this.addCollider)
//            {
//                case MyColliderType.Box:
//                    {
//                        BoxCollider collider = go.AddComponent<BoxCollider>();
//                        if (this.shape == MyShape.Cube || this.shape == MyShape.Top_Front)
//                        {
//                            SetupBoxCollider(collider, texWidth, texHeight, pixelsPerUnit, belowPercent, pivot);
//                        }
//                    }
//                    break;

//                case MyColliderType.Capsule:
//                    go.AddComponent<CapsuleCollider>();
//                    break;
//            }
//        }


//        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
//        {
//            string assetPath = ctx.assetPath;
//            if (assetPath.EndsWith(".cube"))
//            {
//                // load texture
//                string texturePath = ctx.assetPath.Replace(".cube", ".png");
//                this.ImportOne(ctx, texturePath);
//            }
//            else
//            {
//                string dir = Path.GetDirectoryName(assetPath);
//                string[] files = Directory.GetFiles(dir, "*.png");
//                for (int i = 0; i < files.Length; i++)
//                {
//                    string file = files[i];
//                    this.ImportOne(ctx, file);
//                }
//            }
//        }
//    }
//}

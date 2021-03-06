using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Editor
{
    class MyImporterUtils
    {
        const float SQRT2 = 1.4142135623730951f;

        public static void SetupBoxCollider_Cube(BoxCollider collider,
            float texWidth, float texHeight, float pixelsPerUnit, float belowPercent)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = texHeight * belowPercent / pixelsPerUnit;
            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, yLength / 2, zLength / 2);
        }

        public static void SetupBoxCollider_QuadXZ(BoxCollider collider,
            float texWidth, float texHeight, float pixelsPerUnit)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = 0f;// texHeight * belowPercent / pixelsPerUnit;
            float zLength = texHeight * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, 0f, zLength / 2);
        }

        public static void SetupCollider_QuadXY(BoxCollider collider,
            float texWidth, float texHeight, float pixelsPerUnit)
        {
            float xLength = texWidth / pixelsPerUnit;
            //float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float yLength = texHeight / pixelsPerUnit;
            float zLength = 0f;// texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            collider.size = new Vector3(xLength, yLength, zLength);
            collider.center = new Vector3(0f, yLength / 2, zLength / 2);
        }

        public static Mesh CreateMesh_Cube(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent)
        {
            Vector3 pivot = new Vector3(0.5f, 0f, 0f);

            float xLength = texWidth / pixelsPerUnit;
            float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            //Debug.Log(string.Format("length:{0}*{1}*{2}", xLength, yLength, zLength));
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
                vertices[i].x += (0.5f - pivot.x);
                vertices[i].x *= xLength;

                vertices[i].y += (0.5f - pivot.y);
                vertices[i].y *= yLength;

                vertices[i].z += (0.5f - pivot.z);
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
                new Vector2(0f, belowPercent),
                new Vector2(1f, belowPercent),


                new Vector2(1f, belowPercent),
                new Vector2(0f, belowPercent),

                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };

            //if (this.needOptimize)
            //{
            //    MeshUtility.Optimize(mesh);
            //}
            return mesh;
        }

        public static Mesh CreateMesh_TopFront(float texWidth, float texHeight, int pixelsPerUnit, float belowPercent)
        {
            Vector3 pivot = new Vector3(0.5f, 0f, 0f);

            float xLength = texWidth / pixelsPerUnit;
            float yLength = texHeight * belowPercent * SQRT2 / pixelsPerUnit;
            float zLength = texHeight * (1f - belowPercent) * SQRT2 / pixelsPerUnit;

            Mesh mesh = new Mesh();
            var vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),

                // back
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
            };
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].x += (0.5f - pivot.x);
                vertices[i].x *= xLength;

                vertices[i].y += (0.5f - pivot.y);
                vertices[i].y *= yLength;

                vertices[i].z += (0.5f - pivot.z);
                vertices[i].z *= zLength;
            }
            mesh.vertices = vertices;
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
                new Vector2(0f, belowPercent),
                new Vector2(1f, belowPercent),

                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };

            //if (this.needOptimize)
            //{
            //    MeshUtility.Optimize(mesh);
            //}
            return mesh;
        }
        public static Mesh CreateMesh_QuadXZ(float texWidth, float texHeight, int pixelsPerUnit)
        {
            Vector3 pivot = new Vector3(0.5f, 0f, 0f);

            float xLength = texWidth / pixelsPerUnit;
            float zLength = texHeight * SQRT2 / pixelsPerUnit;

            Mesh mesh = new Mesh();
            var vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f),
            };
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].x += (0.5f - pivot.x);
                vertices[i].x *= xLength;

                //vertices[i].y += (0.5f - pivot.y);
                //vertices[i].y *= yLength;

                vertices[i].z += (0.5f - pivot.z);
                vertices[i].z *= zLength;
            }
            mesh.vertices = vertices;
            mesh.triangles = new int[]
            {
                0,2,1,
                1,2,3,
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };
            return mesh;
        }
        public static Mesh CreateMesh_QuadXY(float texWidth, float texHeight, int pixelsPerUnit)
        {
            Vector3 pivot = new Vector3(0.5f, 0f, 0f);

            float xLength = texWidth / pixelsPerUnit;
            float yLength = texHeight * SQRT2 / pixelsPerUnit;

            Mesh mesh = new Mesh();
            var vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f),
            };
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].x += (0.5f - pivot.x);
                vertices[i].x *= xLength;

                vertices[i].y += (0.5f - pivot.y);
                vertices[i].y *= yLength;

                //vertices[i].z += (0.5f - pivot.z);
                //vertices[i].z *= zLength;
            }
            mesh.vertices = vertices;
            mesh.triangles = new int[]
            {
                0,2,1,
                1,2,3,
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };
            return mesh;
        }
        // pts
        // [0]  [1]
        // [2]  [3]
        public static Mesh CreateMesh_Quad_FromPoints(Vector3[] pts, out Vector3 center) // pts.Length=4
        {
            center = (pts[0] + pts[2]) / 2;
            Vector3[] pts2 = new Vector3[4];
            for (int i=0;i<pts.Length;i++)
            {
                pts2[i] = pts[i] - center;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = pts2;
            mesh.triangles = new int[]
            {
                0,1,2,
                2,1,3,
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
            };
            return mesh;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType
{
    Map,
    Floor,
    Stair,
    BoxObstacle,
}

public class EObject : MonoBehaviour
{
    public int Id;
    public EType Type;

    [HideInInspector]
    public BoxCollider collider1;
    [HideInInspector]
    public BoxCollider collider2;

    [HideInInspector]
    public StairDir StairDir;

    [HideInInspector]
    public bool Walkable;

    public LFloorData ToFloorData()
    {
        LFloorData data = new LFloorData();
        data.Id = this.Id;
        data.Y = this.transform.position.y;
        data.Min.y = data.Y;
        data.Max.y = data.Y;

        BoxCollider[] colliders = new BoxCollider[] { this.collider1, this.collider2 };

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (i == 0 || min.x < data.Min.x) data.Min.x = min.x;
            if (i == 0 || min.z < data.Min.z) data.Min.z = min.z;
            if (i == 0 || max.x > data.Max.x) data.Max.x = max.x;
            if (i == 0 || max.z > data.Max.z) data.Max.z = max.z;
        }
        return data;
    }
    public LStairData ToStairData()
    {
        LStairData data = new LStairData();
        data.Id = this.Id;
        data.Dir = this.StairDir;

        BoxCollider[] colliders = new BoxCollider[] { this.collider1, this.collider2 };

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (i == 0 || min.x < data.Min.x) data.Min.x = min.x;
            if (i == 0 || min.y < data.Min.y) data.Min.y = min.y;
            if (i == 0 || min.z < data.Min.z) data.Min.z = min.z;

            if (i == 0 || max.x > data.Max.x) data.Max.x = max.x;
            if (i == 0 || max.y > data.Max.y) data.Max.y = max.y;
            if (i == 0 || max.z > data.Max.z) data.Max.z = max.z;
        }
        return data;
    }
    public LBoxObstacleData ToBoxObstaleData()
    {
        LBoxObstacleData data = new LBoxObstacleData();
        data.Id = this.Id;

        BoxCollider collider = this.GetComponent<BoxCollider>();
        Bounds bounds = collider.bounds;
        data.Min = LVector3.FromVector3(bounds.min);
        data.Max = LVector3.FromVector3(bounds.max);
        return data;
    }
    public LMapData ToMapData()
    {
        LMapData data = new LMapData();
        data.Id = this.Id;

        var Floors = new List<LFloorData>();
        var Stairs = new List<LStairData>();
        var BoxObstacles = new List<LBoxObstacleData>();

        EObject[] objects = this.GetComponentsInChildren<EObject>(false);
        for (int i = 0; i < objects.Length; i++)
        {
            EObject obj = objects[i];
            switch (obj.Type)
            {
                case EType.Floor:
                    Floors.Add(obj.ToFloorData());
                    break;
                case EType.Stair:
                    Stairs.Add(obj.ToStairData());
                    break;
                case EType.BoxObstacle:
                    BoxObstacles.Add(obj.ToBoxObstaleData());
                    break;
            }
        }

        data.Floors = Floors.ToArray();
        data.Stairs = Stairs.ToArray();
        data.BoxObstacles = BoxObstacles.ToArray();

        return data;
    }
}

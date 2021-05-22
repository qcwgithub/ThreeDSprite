using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType
{
    Map,
    Floor,
    Stair,
    BoxObstacle,
    Tree,
}

public enum FloorComposition
{
    ChildrenTopSurfaceRect,
}

public enum BoxObstacleComposition
{
    ChildrenBoxCollidersBox,
    SelfBoxColliderBox,
}

public enum StairComposition
{
    ChildrenBoxColliders,
}


public class EObject : MonoBehaviour
{
    public int Id;
    public EType Type;

    [HideInInspector]
    public StairDir StairDir;

    [HideInInspector]
    public FloorComposition FloorComposition;

    [HideInInspector]
    public BoxObstacleComposition BoxObstacleComposition;

    [HideInInspector]
    public StairComposition StairComposition;

    public LFloorData ToFloorData()
    {
        LFloorData data = new LFloorData();
        data.Id = this.Id;

        switch (this.FloorComposition)
        {
            case FloorComposition.ChildrenTopSurfaceRect:
                {
                    BoxCollider[] colliders = this.GetComponentsInChildren<BoxCollider>(false);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Bounds bound = colliders[i].bounds;
                        Vector3 min = bound.min;
                        Vector3 max = bound.max;

                        if (i == 0 || min.x < data.Min.x) data.Min.x = min.x;
                        if (i == 0 || min.z < data.Min.z) data.Min.z = min.z;
                        if (i == 0 || max.x > data.Max.x) data.Max.x = max.x;
                        if (i == 0 || max.z > data.Max.z) data.Max.z = max.z;

                        if (i == 0)
                        {
                            data.Y = max.y;
                            data.Min.y = data.Y;
                            data.Max.y = data.Y;
                        }
                    }
                }
                break;
        }
        return data;
    }
    public LStairData ToStairData()
    {
        LStairData data = new LStairData();
        data.Id = this.Id;
        data.Dir = this.StairDir;

        switch (this.StairComposition)
        {
            case StairComposition.ChildrenBoxColliders:
                {
                    BoxCollider[] colliders = this.GetComponentsInChildren<BoxCollider>(false);
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
                }
                break;
        }

        return data;
    }
    public LBoxObstacleData ToBoxObstaleData()
    {
        LBoxObstacleData data = new LBoxObstacleData();
        data.Id = this.Id;

        switch (this.BoxObstacleComposition)
        {
            case BoxObstacleComposition.ChildrenBoxCollidersBox:
            case BoxObstacleComposition.SelfBoxColliderBox:
                {
                    BoxCollider[] colliders = 
                        this.BoxObstacleComposition == BoxObstacleComposition.ChildrenBoxCollidersBox 
                        ? this.GetComponentsInChildren<BoxCollider>(false)
                        : new BoxCollider[] { this.GetComponent<BoxCollider>() };
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
                }
                break;
        }

        return data;
    }
    public LTreeData ToTreeData()
    {
        LTreeData data = new LTreeData();
        data.Id = this.Id;

        BoxCollider[] colliders = new BoxCollider[] { this.GetComponent<BoxCollider>() };
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
    public LMapData ToMapData()
    {
        LMapData data = new LMapData();
        data.Id = this.Id;

        var Floors = new List<LFloorData>();
        var Stairs = new List<LStairData>();
        var BoxObstacles = new List<LBoxObstacleData>();
        var Trees = new List<LTreeData>();

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
                case EType.Tree:
                    Trees.Add(obj.ToTreeData());
                    break;
            }
        }

        data.Floors = Floors.ToArray();
        data.Stairs = Stairs.ToArray();
        data.BoxObstacles = BoxObstacles.ToArray();
        data.Trees = Trees.ToArray();

        return data;
    }
}

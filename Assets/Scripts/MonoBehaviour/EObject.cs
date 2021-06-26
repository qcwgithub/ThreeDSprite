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
/*
    public btFloorData ToFloorData()
    {
        btFloorData data = new btFloorData();
        data.id = this.Id;

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

                        if (i == 0 || min.x < data.min.x) data.min.x = min.x;
                        if (i == 0 || min.z < data.min.z) data.min.z = min.z;
                        if (i == 0 || max.x > data.max.x) data.max.x = max.x;
                        if (i == 0 || max.z > data.max.z) data.max.z = max.z;

                        if (i == 0)
                        {
                            data.y = max.y;
                            data.min.y = data.y;
                            data.max.y = data.y;
                        }
                    }
                }
                break;
        }
        return data;
    }
    public btStairData ToStairData()
    {
        btStairData data = new btStairData();
        data.id = this.Id;
        data.dir = this.StairDir;

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

                        if (i == 0 || min.x < data.min.x) data.min.x = min.x;
                        if (i == 0 || min.y < data.min.y) data.min.y = min.y;
                        if (i == 0 || min.z < data.min.z) data.min.z = min.z;

                        if (i == 0 || max.x > data.max.x) data.max.x = max.x;
                        if (i == 0 || max.y > data.max.y) data.max.y = max.y;
                        if (i == 0 || max.z > data.max.z) data.max.z = max.z;
                    }
                }
                break;
        }

        return data;
    }
    public btBoxObstacleData ToBoxObstaleData()
    {
        btBoxObstacleData data = new btBoxObstacleData();
        data.id = this.Id;

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

                        if (i == 0 || min.x < data.min.x) data.min.x = min.x;
                        if (i == 0 || min.y < data.min.y) data.min.y = min.y;
                        if (i == 0 || min.z < data.min.z) data.min.z = min.z;

                        if (i == 0 || max.x > data.max.x) data.max.x = max.x;
                        if (i == 0 || max.y > data.max.y) data.max.y = max.y;
                        if (i == 0 || max.z > data.max.z) data.max.z = max.z;
                    }
                }
                break;
        }

        return data;
    }
    public btTreeData ToTreeData()
    {
        btTreeData data = new btTreeData();
        data.id = this.Id;

        BoxCollider[] colliders = new BoxCollider[] { this.GetComponent<BoxCollider>() };
        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (i == 0 || min.x < data.min.x) data.min.x = min.x;
            if (i == 0 || min.y < data.min.y) data.min.y = min.y;
            if (i == 0 || min.z < data.min.z) data.min.z = min.z;

            if (i == 0 || max.x > data.max.x) data.max.x = max.x;
            if (i == 0 || max.y > data.max.y) data.max.y = max.y;
            if (i == 0 || max.z > data.max.z) data.max.z = max.z;
        }

        return data;
    }
    public btSceneData ToMapData()
    {
        btSceneData data = new btSceneData();
        data.id = this.Id;

        var Floors = new List<btFloorData>();
        var Stairs = new List<btStairData>();
        var BoxObstacles = new List<btBoxObstacleData>();
        var Trees = new List<btTreeData>();

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

        data.floors = Floors.ToArray();
        data.stairs = Stairs.ToArray();
        data.boxObstacles = BoxObstacles.ToArray();
        data.trees = Trees.ToArray();

        return data;
    }
    */
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class btFloor : btObject, btIWalkable
{
    public btFloorData Data { get; private set; }
    public btFloor(btScene scene, btFloorData data): base(scene, data.id)
    {
        this.Data = data;
    }
    public override btObjectType Type { get { return btObjectType.floor; } }

    //public override int Priority { get { return 1; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        btFloorData data = this.Data;

        bool outOfRange = false;
        if (pos.x < data.min.x)
        {
            pos.x = data.min.x;
            outOfRange = true;
        }
        else if (pos.x > data.max.x)
        {
            pos.x = data.max.x;
            outOfRange = true;
        }

        if (pos.z < data.min.z)
        {
            pos.z = data.min.z;
            outOfRange = true;
        }
        else if (pos.z > data.max.z)
        {
            pos.z = data.max.z;
            outOfRange = true;
        }
        return outOfRange;
    }

    // public Vector3 RandomPos()
    // {
    //     LFloorData data = this.Data;
    //     return new Vector3(UnityEngine.Random.Range(data.Min.x, data.Max.x), data.Y, UnityEngine.Random.Range(data.Min.z, data.Max.z));
    // }

    public PredictMoveResult PredictMove(Vector3 from, Vector3 delta)
    {
        btFloorData data = this.Data;
        PredictMoveResult result = default;
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            result.OutOfRange = true;
            return result;
        }

        result.Y = data.y;
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        btFloorData data = this.Data;
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }
        if (delta.y < 0 && from.y > data.y && to.y <= data.y)
        {
            return true;
        }
        if (Mathf.Abs(data.y - to.y) > 0.1f)
        {
            return false;
        }
        return true;
    }

    public override void AddToPhysicsScene()
    {
        Vector3 min = FVector3.ToVector3(this.Data.min);
        Vector3 max = FVector3.ToVector3(this.Data.max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

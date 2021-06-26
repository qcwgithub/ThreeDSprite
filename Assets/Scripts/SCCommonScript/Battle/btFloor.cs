using System;
using System.Collections.Generic;
using UnityEngine;

public class btFloor : btObject, btIWalkable
{
    public float y;
    public Vector3 min;
    public Vector3 max;
    public btFloor(btScene scene, int id, Vector3 min, Vector3 max): base(scene, id)
    {
        this.min = min;
        this.max = max;
        this.y = min.y;
    }
    public override btObjectType Type { get { return btObjectType.floor; } }

    //public override int Priority { get { return 1; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        bool outOfRange = false;
        if (pos.x < min.x)
        {
            pos.x = min.x;
            outOfRange = true;
        }
        else if (pos.x > max.x)
        {
            pos.x = max.x;
            outOfRange = true;
        }

        if (pos.z < min.z)
        {
            pos.z = min.z;
            outOfRange = true;
        }
        else if (pos.z > max.z)
        {
            pos.z = max.z;
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
        PredictMoveResult result = default;
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            result.OutOfRange = true;
            return result;
        }

        result.Y = this.y;
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }
        if (delta.y < 0 && from.y > this.y && to.y <= this.y)
        {
            return true;
        }
        if (Mathf.Abs(this.y - to.y) > 0.1f)
        {
            return false;
        }
        return true;
    }

    public override void AddToPhysicsScene()
    {
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

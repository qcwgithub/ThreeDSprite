using System;
using System.Collections.Generic;
using UnityEngine;

public class btLFloor : btObject, btIWalkable
{
    public btFloorData Data { get; private set; }
    public btLFloor(btScene scene, btFloorData data): base(scene, data.Id)
    {
        this.Data = data;
    }
    public override btObjectType Type { get { return btObjectType.Floor; } }

    //public override int Priority { get { return 1; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        btFloorData data = this.Data;

        bool outOfRange = false;
        if (pos.x < data.Min.x)
        {
            pos.x = data.Min.x;
            outOfRange = true;
        }
        else if (pos.x > data.Max.x)
        {
            pos.x = data.Max.x;
            outOfRange = true;
        }

        if (pos.z < data.Min.z)
        {
            pos.z = data.Min.z;
            outOfRange = true;
        }
        else if (pos.z > data.Max.z)
        {
            pos.z = data.Max.z;
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

        result.Y = data.Y;
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
        if (delta.y < 0 && from.y > data.Y && to.y <= data.Y)
        {
            return true;
        }
        if (Mathf.Abs(data.Y - to.y) > 0.1f)
        {
            return false;
        }
        return true;
    }

    public override void AddToPhysicsScene()
    {
        Vector3 min = LVector3.ToVector3(this.Data.Min);
        Vector3 max = LVector3.ToVector3(this.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        scene.AddBox(this.body, Vector3.zero, size/2);
    }
}
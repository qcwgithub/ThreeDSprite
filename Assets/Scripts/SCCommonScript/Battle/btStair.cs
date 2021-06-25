using System;
using System.Collections.Generic;
using UnityEngine;

public enum StairDir
{
    Front_Back,
    LeftHigh_RightLow,
    LeftLow_RightHigh,
}
public class btStair : btObject, btIWalkable
{
    public btStairData Data { get; private set; }
    public btStair(btScene scene, btStairData data) : base(scene, data.id)
    {
        this.Data = data;
    }
    public override btObjectType Type { get { return btObjectType.stair; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        btStairData data = this.Data;
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

    private float ZtoY(float z)
    {
        btStairData data = this.Data;
        float t = (z - data.min.z) / (data.max.z - data.min.z);
        float y = UnityEngine.Mathf.Lerp(data.min.y, data.max.y, t);
        return y;
    }

    private float XtoY(float x, StairDir dir)
    {
        btStairData data = this.Data;
        if (dir == StairDir.LeftLow_RightHigh)
        {
            float t = (x - data.min.x) / (data.max.x - data.min.x);
            float y = UnityEngine.Mathf.Lerp(data.min.y, data.max.y, t);
            return y;
        }
        else
        {
            float t = (x - data.min.x) / (data.max.x - data.min.x);
            float y = UnityEngine.Mathf.Lerp(data.max.y, data.min.y, t);
            return y;
        }
    }

    private float XZtoY(float x, float z)
    {
        btStairData data = this.Data;
        switch (data.dir)
        {
            case StairDir.Front_Back:
                return this.ZtoY(z);
                //break;
            case StairDir.LeftHigh_RightLow:
            case StairDir.LeftLow_RightHigh:
            default:
                return this.XtoY(x, data.dir);
                //break;
        }
    }

    // public Vector3 RandomPos()
    // {
    //     LStairData data = this.Data;
    //     Vector3 pos =  new Vector3(UnityEngine.Random.Range(data.Min.x, data.Max.x), 
    //         0f, 
    //         UnityEngine.Random.Range(data.Min.z, data.Max.z));

    //     pos.y = this.ZtoY(pos.z);
    //     return pos;
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

        result.Y = this.XZtoY(to.x, to.z);
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }

        float y = this.XZtoY(to.x, to.z);
        if (delta.y < 0 && from.y > y && to.y <= y)
        {
            return true;
        }
        if (Mathf.Abs(to.y - y) > 0.1f)
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
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

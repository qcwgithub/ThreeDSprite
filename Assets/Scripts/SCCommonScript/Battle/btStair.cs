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
    public btStair(btScene scene, btStairData data) : base(scene, data.Id)
    {
        this.Data = data;
    }
    public override btObjectType Type { get { return btObjectType.stair; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        btStairData data = this.Data;
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

    private float ZtoY(float z)
    {
        btStairData data = this.Data;
        float t = (z - data.Min.z) / (data.Max.z - data.Min.z);
        float y = UnityEngine.Mathf.Lerp(data.Min.y, data.Max.y, t);
        return y;
    }

    private float XtoY(float x, StairDir dir)
    {
        btStairData data = this.Data;
        if (dir == StairDir.LeftLow_RightHigh)
        {
            float t = (x - data.Min.x) / (data.Max.x - data.Min.x);
            float y = UnityEngine.Mathf.Lerp(data.Min.y, data.Max.y, t);
            return y;
        }
        else
        {
            float t = (x - data.Min.x) / (data.Max.x - data.Min.x);
            float y = UnityEngine.Mathf.Lerp(data.Max.y, data.Min.y, t);
            return y;
        }
    }

    private float XZtoY(float x, float z)
    {
        btStairData data = this.Data;
        switch (data.Dir)
        {
            case StairDir.Front_Back:
                return this.ZtoY(z);
                //break;
            case StairDir.LeftHigh_RightLow:
            case StairDir.LeftLow_RightHigh:
            default:
                return this.XtoY(x, data.Dir);
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
        Vector3 min = LVector3.ToVector3(this.Data.Min);
        Vector3 max = LVector3.ToVector3(this.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

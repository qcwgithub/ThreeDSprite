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
    public Vector3 min;
    public Vector3 max;
    public StairDir dir;
    public btStair(btScene scene, int id, StairDir dir, Vector3 min, Vector3 max) : base(scene, id)
    {
        this.min = min;
        this.max = max;
        this.dir = dir;
    }
    public override btObjectType Type { get { return btObjectType.stair; } }

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

    private float ZtoY(float z)
    {
        float t = (z - min.z) / (max.z - min.z);
        float y = UnityEngine.Mathf.Lerp(min.y, max.y, t);
        return y;
    }

    private float XtoY(float x, StairDir dir)
    {
        if (dir == StairDir.LeftLow_RightHigh)
        {
            float t = (x - min.x) / (max.x - min.x);
            float y = UnityEngine.Mathf.Lerp(min.y, max.y, t);
            return y;
        }
        else
        {
            float t = (x - min.x) / (max.x - min.x);
            float y = UnityEngine.Mathf.Lerp(max.y, min.y, t);
            return y;
        }
    }

    private float XZtoY(float x, float z)
    {
        switch (dir)
        {
            case StairDir.Front_Back:
                return this.ZtoY(z);
                //break;
            case StairDir.LeftHigh_RightLow:
            case StairDir.LeftLow_RightHigh:
            default:
                return this.XtoY(x, dir);
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
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

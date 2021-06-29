using System;
using System.Collections.Generic;
using UnityEngine;

public enum StairDir
{
    front_back,
    left_high_right_low,
    left_low_right_high,
}
public class btStair : btObject, btIWalkable
{
    public Vector3 worldMin;
    public Vector3 worldMax;
    public StairDir dir;
    public btStair(btScene scene, int id, StairDir dir, Vector3 worldMin, Vector3 worldMax) : base(scene, id)
    {
        this.worldMin = worldMin;
        this.worldMax = worldMax;
        this.dir = dir;
    }
    public override btObjectType Type { get { return btObjectType.stair; } }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        bool outOfRange = false;
        if (pos.x < worldMin.x)
        {
            pos.x = worldMin.x;
            outOfRange = true;
        }
        else if (pos.x > worldMax.x)
        {
            pos.x = worldMax.x;
            outOfRange = true;
        }

        if (pos.z < worldMin.z)
        {   
            pos.z = worldMin.z;
            outOfRange = true;
        }
        else if (pos.z > worldMax.z)
        {
            pos.z = worldMax.z;
            outOfRange = true;
        }
        return outOfRange;
    }

    private float ZtoY(float z)
    {
        float t = (z - worldMin.z) / (worldMax.z - worldMin.z);
        float y = UnityEngine.Mathf.Lerp(worldMin.y, worldMax.y, t);
        return y;
    }

    private float XtoY(float x, StairDir dir)
    {
        if (dir == StairDir.left_low_right_high)
        {
            float t = (x - worldMin.x) / (worldMax.x - worldMin.x);
            float y = UnityEngine.Mathf.Lerp(worldMin.y, worldMax.y, t);
            return y;
        }
        else
        {
            float t = (x - worldMin.x) / (worldMax.x - worldMin.x);
            float y = UnityEngine.Mathf.Lerp(worldMax.y, worldMin.y, t);
            return y;
        }
    }

    private float XZtoY(float x, float z)
    {
        switch (dir)
        {
            case StairDir.front_back:
                return this.ZtoY(z);
                //break;
            case StairDir.left_high_right_low:
            case StairDir.left_low_right_high:
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
        Vector3 center = (worldMin + worldMax) / 2;
        Vector3 size = worldMax - worldMin;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBoxObstacle : LObject, IObstacle, IWalkable
{
    public LBoxObstacleData Data { get; private set; }
    public float Y { get; private set; }
    public LBoxObstacle(LBoxObstacleData data): base(data.Id)
    {
        this.Data = data;
        this.Y = data.Max.y;
    }

    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        LBoxObstacleData data = this.Data;
        Vector3 to = from + delta;
        if (to.x < data.Min.x || to.x > data.Max.x || to.z < data.Min.z || to.z > data.Max.z)
        {
            return false;
        }

        bool ret = false;
        if (from.x <= data.Min.x && delta.x > 0 && to.x > data.Min.x)
        {
            delta.x = 0;
            ret = true;
        }
        else if (from.x >= data.Max.x && delta.x < 0 && to.x < data.Max.x)
        {
            delta.x = 0;
            ret = true;
        }
        if (from.z <= data.Min.z && delta.z > 0 && to.z > data.Min.z)
        {
            delta.z = 0;
            ret = true;
        }
        else if (from.z >= data.Max.z && delta.z < 0 && to.z < data.Max.z)
        {
            delta.z = 0;
            ret = true;
        }
        return ret;
    }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        LBoxObstacleData data = this.Data;
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

    public Vector3 RandomPos()
    {
        LBoxObstacleData data = this.Data;
        return new Vector3(UnityEngine.Random.Range(data.Min.x, data.Max.x), this.Y, UnityEngine.Random.Range(data.Min.z, data.Max.z));
    }

    public PredictMoveResult PredictMove(Vector3 from, Vector3 delta)
    {
        LBoxObstacleData data = this.Data;
        PredictMoveResult result = default;
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            result.OutOfRange = true;
            return result;
        }

        result.Y = this.Y;
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        if (!this.Data.Walkable)
        {
            return false;
        }
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }
        if (delta.y < 0 && from.y > this.Y && to.y <= this.Y)
        {
            return true;
        }
        if (Mathf.Abs(this.Y - to.y) > 0.1f)
        {
            return false;
        }
        return true;
    }
}

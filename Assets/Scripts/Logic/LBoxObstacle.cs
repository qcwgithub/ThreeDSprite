using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBoxObstacle : LObject, IObstacle
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
}

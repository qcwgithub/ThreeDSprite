using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btBoxObstacle : btObject, btIObstacle
{
    public btBoxObstacleData Data { get; private set; }
    public float Y { get; private set; }
    public btBoxObstacle(btScene scene, btBoxObstacleData data): base(scene, data.Id)
    {
        this.Data = data;
        this.Y = data.Max.y;
    }
    public override btObjectType Type { get { return btObjectType.box_obstacle; } }

    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        btBoxObstacleData data = this.Data;
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

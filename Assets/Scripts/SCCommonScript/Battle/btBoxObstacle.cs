using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btBoxObstacle : btObject, btIObstacle
{
    public btBoxObstacleData Data { get; private set; }
    public float Y { get; private set; }
    public btBoxObstacle(btScene scene, btBoxObstacleData data): base(scene, data.id)
    {
        this.Data = data;
        this.Y = data.max.y;
    }
    public override btObjectType Type { get { return btObjectType.box_obstacle; } }

    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        btBoxObstacleData data = this.Data;
        Vector3 to = from + delta;
        if (to.x < data.min.x || to.x > data.max.x || to.z < data.min.z || to.z > data.max.z)
        {
            return false;
        }

        bool ret = false;
        if (from.x <= data.min.x && delta.x > 0 && to.x > data.min.x)
        {
            delta.x = 0;
            ret = true;
        }
        else if (from.x >= data.max.x && delta.x < 0 && to.x < data.max.x)
        {
            delta.x = 0;
            ret = true;
        }
        if (from.z <= data.min.z && delta.z > 0 && to.z > data.min.z)
        {
            delta.z = 0;
            ret = true;
        }
        else if (from.z >= data.max.z && delta.z < 0 && to.z < data.max.z)
        {
            delta.z = 0;
            ret = true;
        }
        return ret;
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

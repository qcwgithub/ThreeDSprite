using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btBoxObstacle : btObject, btIObstacle
{
    public btTileData data { get; private set; }
    public btTileConfig thingConfig;
    // public float Y { get; private set; }
    public Vector3 min;
    public Vector3 max;
    public btBoxObstacle(btScene scene, btTileData data, btTileConfig config): base(scene, data.id)
    {
        this.data = data;
        this.thingConfig= config;
        // this.Y = data.max.y;
        this.min = FVector3.ToVector3(this.data.position);
        this.max = this.min + FVector3.ToVector3(thingConfig.size);
    }
    public override btObjectType Type { get { return btObjectType.box_obstacle; } }

    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        btTileData data = this.data;
        Vector3 to = from + delta;
        if (to.x < min.x || to.x > max.x || to.z < min.z || to.z > max.z)
        {
            return false;
        }

        bool ret = false;
        if (from.x <= min.x && delta.x > 0 && to.x > min.x)
        {
            delta.x = 0;
            ret = true;
        }
        else if (from.x >= max.x && delta.x < 0 && to.x < max.x)
        {
            delta.x = 0;
            ret = true;
        }
        if (from.z <= min.z && delta.z > 0 && to.z > min.z)
        {
            delta.z = 0;
            ret = true;
        }
        else if (from.z >= max.z && delta.z < 0 && to.z < max.z)
        {
            delta.z = 0;
            ret = true;
        }
        return ret;
    }


    public override void AddToPhysicsScene()
    {
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;

        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

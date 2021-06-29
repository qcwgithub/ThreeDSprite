using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btBoxObstacle : btObject, btIObstacle
{
    public btTileData data { get; private set; }
    public btTileConfig tileConfig;
    // public float Y { get; private set; }
    public Vector3 worldMin;
    public Vector3 worldMax;
    public btBoxObstacle(btScene scene, Vector3 parentOffset, btTileData data, btTileConfig config): base(scene, data.id)
    {
        this.data = data;
        this.tileConfig= config;
        // this.Y = data.max.y;
        this.worldMin = FVector3.ToVector3(this.data.position) + parentOffset;
        this.worldMax = this.worldMin + FVector3.ToVector3(tileConfig.size);
    }
    public override btObjectType Type { get { return btObjectType.box_obstacle; } }

    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        btTileData data = this.data;
        Vector3 to = from + delta;
        if (to.x < worldMin.x || to.x > worldMax.x || to.z < worldMin.z || to.z > worldMax.z)
        {
            return false;
        }

        bool ret = false;
        if (from.x <= worldMin.x && delta.x > 0 && to.x > worldMin.x)
        {
            delta.x = 0;
            ret = true;
        }
        else if (from.x >= worldMax.x && delta.x < 0 && to.x < worldMax.x)
        {
            delta.x = 0;
            ret = true;
        }
        if (from.z <= worldMin.z && delta.z > 0 && to.z > worldMin.z)
        {
            delta.z = 0;
            ret = true;
        }
        else if (from.z >= worldMax.z && delta.z < 0 && to.z < worldMax.z)
        {
            delta.z = 0;
            ret = true;
        }
        return ret;
    }


    public override void AddToPhysicsScene()
    {
        Vector3 center = (worldMin + worldMax) / 2;
        Vector3 size = worldMax - worldMin;

        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

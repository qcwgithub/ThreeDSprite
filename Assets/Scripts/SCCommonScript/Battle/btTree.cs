using System;
using System.Collections.Generic;
using UnityEngine;

public class btTree : btObject
{
    public btTileData data { get; private set; }
    public btTileConfig tileConfig;
    public Vector3 worldMin;
    public Vector3 worldMax;
    public btTree(btScene scene, Vector3 parentOffset, btTileData data, btTileConfig config) : base(scene, data.id)
    {
        this.data = data;
        this.tileConfig = config;
        this.worldMin = FVector3.ToVector3(this.data.position) + parentOffset;
        this.worldMax = this.worldMin + FVector3.ToVector3(tileConfig.size);
    }
    public override btObjectType Type { get { return btObjectType.tree; } }

    public override void AddToPhysicsScene()
    {
        Vector3 center = (worldMin + worldMax) / 2;
        Vector3 size = worldMax - worldMin;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class btTree : btObject
{
    public btTileData data { get; private set; }
    public btTileConfig thingConfig;
    public Vector3 min;
    public Vector3 max;
    public btTree(btScene scene, btTileData data, btTileConfig config) : base(scene, data.id)
    {
        this.data = data;
        this.thingConfig = config;
        this.min = FVector3.ToVector3(this.data.position);
        this.max = this.min + FVector3.ToVector3(thingConfig.size);
    }
    public override btObjectType Type { get { return btObjectType.tree; } }

    public override void AddToPhysicsScene()
    {
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

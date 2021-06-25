using System;
using System.Collections.Generic;
using UnityEngine;

public class btTree : btObject
{
    public btTreeData Data { get; private set; }
    public btTree(btScene scene, btTreeData data) : base(scene, data.Id)
    {
        this.Data = data;
    }
    public override btObjectType Type { get { return btObjectType.tree; } }

    public override void AddToPhysicsScene()
    {
        Vector3 min = LVector3.ToVector3(this.Data.Min);
        Vector3 max = LVector3.ToVector3(this.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        
        this.body = scene.AddBody(this, q3BodyType.eStaticBody, center);
        this.scene.AddBox(this.body, Vector3.zero, size/2);
    }
}

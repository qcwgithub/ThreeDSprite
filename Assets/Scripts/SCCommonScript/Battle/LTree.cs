using System;
using System.Collections.Generic;
using UnityEngine;

public class LTree : LObject
{
    public LTreeData Data { get; private set; }
    public LTree(LMap lMap, LTreeData data) : base(lMap, data.Id)
    {
        this.Data = data;
    }
    public override LObjectType Type { get { return LObjectType.Tree; } }

    public override void AddToOctree(BoundsOctree<LObject> octree)
    {
        Vector3 min = LVector3.ToVector3(this.Data.Min);
        Vector3 max = LVector3.ToVector3(this.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;

        octree.Add(this, new Bounds(center, size));
    }
}

using System.Collections;
using System.Collections.Generic;

public abstract class LObject
{
    public int Id { get; private set; }
    public LMap lMap { get; private set; }
    public LObject(LMap lMap, int id)
    {
        this.lMap = lMap;
        this.Id = id;
    }
    public abstract LObjectType Type { get; }
    public virtual void AddToOctree(BoundsOctree<LObject> octree)
    {

    }

    public override string ToString()
    {
        return this.Type.ToString() + this.Id;
    }
    public virtual void Update()
    {

    }
}

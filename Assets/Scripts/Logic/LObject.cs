using System.Collections;
using System.Collections.Generic;

public abstract class LObject
{
    public int Id { get; private set; }
    public LObject(int id)
    {
        this.Id = id;
    }
    public abstract LObjectType Type { get; }
    public virtual void AddToOctree(BoundsOctree<LObject> octree)
    {

    }
}

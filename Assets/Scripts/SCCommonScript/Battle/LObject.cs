using System;
using System.Collections;
using System.Collections.Generic;

public abstract class LObject
{
    public int Id { get; private set; }
    public LMap lMap { get; private set; }
    public IntPtr body = IntPtr.Zero;
    // public IntPtr box = IntPtr.Zero;
    public List<LObject_Time> Collidings { get; } = new List<LObject_Time>();

    public LObject(LMap lMap, int id)
    {
        this.lMap = lMap;
        this.Id = id;
    }
    public abstract LObjectType Type { get; }
    public virtual void AddToPhysicsScene()
    {

    }

    public override string ToString()
    {
        return this.Type.ToString() + this.Id;
    }
    // public virtual void Update()
    // {

    // }
}

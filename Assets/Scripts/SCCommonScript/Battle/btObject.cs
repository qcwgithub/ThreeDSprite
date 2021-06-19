using System;
using System.Collections;
using System.Collections.Generic;

public abstract class btObject
{
    public int Id { get; private set; }
    public btScene scene { get; private set; }
    public IntPtr body = IntPtr.Zero;
    // public IntPtr box = IntPtr.Zero;
    public List<btObject_Time> Collidings { get; } = new List<btObject_Time>();

    public btObject(btScene scene, int id)
    {
        this.scene = scene;
        this.Id = id;
    }
    public abstract btObjectType Type { get; }
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

using System;
using System.Collections;
using System.Collections.Generic;

public abstract class btObject
{
    public int id { get; private set; }
    public btScene scene { get; private set; }
    public IntPtr body = IntPtr.Zero;
    // public IntPtr box = IntPtr.Zero;
    public List<btObject_Time> Collidings { get; } = new List<btObject_Time>();

    public btObject(btScene scene, int id)
    {
        this.scene = scene;
        this.id = id;
    }
    public abstract btObjectType Type { get; }
    public virtual void AddToPhysicsScene()
    {

    }

    public override string ToString()
    {
        return this.Type.ToString() + this.id;
    }
    // public virtual void Update()
    // {

    // }
}

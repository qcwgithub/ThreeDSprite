using System;
using System.Collections.Generic;
using UnityEngine;

public class LCharacter : LObject
{
    public LCharacter(int id) : base(id)
    {

    }
    public override LObjectType Type { get { return LObjectType.Character; } }
    public event Action<Vector3> PosChanged;

    private IWalkable walkable;
    public IWalkable Walkable
    {
        get { return this.walkable; }
        set
        {
            IWalkable pre = this.walkable;
            if (pre == value)
            {
                return;
            }
            this.walkable = value;
            //Debug.Log(string.Format("{0} -> {1}", 
            //    pre == null ? "null" : pre.Id.ToString(), 
            //    value == null ? "null" : value.Id.ToString()));
        }
    }

    private Vector3 pos;
    public Vector3 Pos
    {
        get
        {
            return this.pos;
        }
        set
        {
            this.pos = value;
            if (this.PosChanged != null)
            {
                this.PosChanged(value);
            }
        }
    }

    public override void AddToOctree(BoundsOctree<LObject> octree)
    {
        
    }
}

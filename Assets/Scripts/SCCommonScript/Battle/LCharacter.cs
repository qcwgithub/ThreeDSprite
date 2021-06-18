using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public struct LObject_Time
{
    public LObject obj;
    public float time;
}


public class LCharacter : LObject
{
    public LCharacter(LMap lMap, int id) : base(lMap, id)
    {
        lMap.AddNeedUpdate(this.Id);
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
            Debug.Log(string.Format("{0} -> {1}",
                pre == null ? "null" : pre.ToString(),
                value == null ? "null" : value.ToString()));
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

            if (this.body != IntPtr.Zero)
                lMap.SetBodyPosition(this.body, value);

            if (this.PosChanged != null)
            {
                this.PosChanged(value);
            }
        }
    }

    public override void AddToPhysicsScene()
    {
        this.body = lMap.AddBody(this, q3BodyType.eDynamicBody, this.pos + new Vector3(0f, 0.4f, 0f));
        lMap.AddBox(this.body, Vector3.zero, new Vector3(0.2f, 0.4f, 0.2f));
    }
}

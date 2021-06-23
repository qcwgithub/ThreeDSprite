using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public struct btObject_Time
{
    public btObject obj;
    public float time;
}


public class btCharacter : btObject
{
    public btCharacter(btScene scene, int id) : base(scene, id)
    {
        scene.AddNeedUpdate(this.Id);
    }
    public override btObjectType Type { get { return btObjectType.Character; } }
    public event Action<Vector3> PosChanged;

    private btIWalkable walkable;
    public btIWalkable Walkable
    {
        get { return this.walkable; }
        set
        {
            btIWalkable pre = this.walkable;
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
                scene.SetBodyPosition(this.body, value);

            if (this.PosChanged != null)
            {
                this.PosChanged(value);
            }
        }
    }

    public override void AddToPhysicsScene()
    {
        this.body = scene.AddBody(this, q3BodyType.eDynamicBody, this.pos + new Vector3(0f, 0.4f, 0f));
        scene.AddBox(this.body, Vector3.zero, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
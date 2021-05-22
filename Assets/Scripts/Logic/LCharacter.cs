using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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

    private List<LObject> colliding1 = new List<LObject>();
    public List<LObject> Collidings { get; } = new List<LObject>();
    public override void Update()
    {
        Bounds bounds = new Bounds(this.Pos, new Vector3(0.4f, 0.4f, 0.4f));
        this.colliding1.Clear();
        this.lMap.Octree.GetColliding(this.colliding1, bounds);

        bool different = this.colliding1.Count != this.Collidings.Count;
        if (!different)
        {
            for (int i = 0; i < this.colliding1.Count; i++)
            {
                if (this.colliding1[i].Id != this.Collidings[i].Id)
                {
                    different = true;
                    break;
                }
            }
        }
        if (different)
        {
            var sb = new StringBuilder();
            sb.Append("Collides with: ");
            for (int i = 0; i < this.colliding1.Count; i++)
            {
                sb.AppendFormat("{0}{1}", this.colliding1[i].Type, this.colliding1[i].Id);
                if (i < this.colliding1.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            Debug.Log(sb.ToString());
            //
            this.Collidings.Clear();
            this.Collidings.AddRange(this.colliding1);
        }
    }
}

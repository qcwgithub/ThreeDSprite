using System;
using System.Collections.Generic;
using UnityEngine;

public class CCharacter : CObject
{
    public Spine.Unity.SkeletonAnimation Skel;
    public float Speed = 5f;

    private CWalkable walkable;
    public CWalkable Walkable
    { 
        get { return this.walkable;  } 
        set
        {
            CWalkable pre = this.walkable;
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

    [NonSerialized]
    public List<CObstacle> ListObstacles = new List<CObstacle>();

    public event Action<CCharacter> PosChanged;
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
            this.transform.position = this.pos;
            if (this.PosChanged != null)
            {
                this.PosChanged(this);
            }
        }
    }

    public event Action<CCharacter, Collider> _OnTriggerEnter;
    public event Action<CCharacter, Collider> _OnTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        if (this._OnTriggerEnter != null)
        {
            this._OnTriggerEnter(this, other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (this._OnTriggerExit != null)
        {
            this._OnTriggerExit(this, other);
        }
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0f || z != 0f)
        {
            this.Skel.AnimationName = "run";
            this.Skel.loop = true;

            Vector3 delta = this.Speed * Time.deltaTime * new Vector3(x, 0f, z);
            this.Walkable.Move(this, delta);
        }
        else
        {
            this.Skel.AnimationName = "idle";
            this.Skel.loop = true;
        }
    }
}

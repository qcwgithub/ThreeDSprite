using System;
using System.Collections.Generic;
using UnityEngine;

public class CCharacter : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    public float Speed = 5f;

    private IWalkable walkable;
    public IWalkable Walkable
    { 
        get { return this.walkable;  } 
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

    public void Apply()
    {
        this.pos = this.transform.position;
    }

    [NonSerialized]
    public List<IObstacle> ListObstacles = new List<IObstacle>();

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
}

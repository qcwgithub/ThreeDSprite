using System;
using System.Collections.Generic;
using UnityEngine;

public class CCharacter : MonoBehaviour
{
    private CWalkable currWalkable;
    public CWalkable CurrWalkable
    { 
        get { return this.currWalkable;  } 
        set
        {
            CWalkable pre = this.currWalkable;
            if (pre == value)
            {
                return;
            }
            this.currWalkable = value;
            Debug.Log(string.Format("{0} -> {1}", 
                pre == null ? "null" : pre.Id.ToString(), 
                value == null ? "null" : value.Id.ToString()));
        }
    }
    [NonSerialized]
    public List<CWalkable> ListWalkables = new List<CWalkable>();

    public event Action<CCharacter, Collider> ActionOnTriggerEnter;
    public event Action<CCharacter, Collider> ActionOnTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        this.ActionOnTriggerEnter?.Invoke(this, other);
    }
    private void OnTriggerExit(Collider other)
    {
        this.ActionOnTriggerExit?.Invoke(this, other);
    }
}

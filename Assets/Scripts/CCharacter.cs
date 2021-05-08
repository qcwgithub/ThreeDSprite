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
            this.currWalkable = value;
            Debug.Log(string.Format("{0} -> {1}", 
                pre == null ? "null" : pre.Id.ToString(), 
                value == null ? "null" : value.Id.ToString()));
        }
    }

    public event Action<CCharacter, Collider> ActionOnTriggerEnter;
    public event Action<CCharacter, Collider> ActionOnTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnCollisionEnter");
        if (this.ActionOnTriggerEnter != null)
        {
            this.ActionOnTriggerEnter(this, other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnCollisionExit");
        if (this.ActionOnTriggerExit != null)
        {
            this.ActionOnTriggerExit(this, other);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObject : MonoBehaviour
{
    public int Id;
    public LObject lObj { get; protected set; }
    public virtual void Apply(LObject lObj)
    {
        this.lObj = lObj;
    }
}

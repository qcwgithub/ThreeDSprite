using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObject : MonoBehaviour
{
    public int Id;
    public LObject lObj { get; protected set; }
    public CMap cMap { get; protected set; }
    public virtual void Apply(CMap cMap, LObject lObj)
    {
        this.cMap = cMap;
        this.lObj = lObj;
    }
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (!this.cMap.DrawGizmos_ObjectBounds)
        {
            return;
        }
        this.OnDrawGizmosImpl();
    }
    protected virtual void OnDrawGizmosImpl()
    {

    }
#endif
}

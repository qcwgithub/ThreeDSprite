using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFloor : CObject
{
    protected LFloor lFloor;
    public override void Apply(CMap cMap, LObject lObj)
    {
        base.Apply(cMap, lObj);
        this.lFloor = lObj as LFloor;
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        Vector3 min = LVector3.ToVector3(this.lFloor.Data.Min);
        Vector3 max = LVector3.ToVector3(this.lFloor.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

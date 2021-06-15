using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTree : CObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        LTree lTree = this.lObj as LTree;
        Vector3 min = LVector3.ToVector3(lTree.Data.Min);
        Vector3 max = LVector3.ToVector3(lTree.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtTree : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btTree lTree = this.obj as btTree;
        Vector3 min = LVector3.ToVector3(lTree.Data.Min);
        Vector3 max = LVector3.ToVector3(lTree.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

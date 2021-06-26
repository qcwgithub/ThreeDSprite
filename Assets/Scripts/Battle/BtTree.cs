using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtTree : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btTree lTree = this.obj as btTree;
        Vector3 center = (lTree.min + lTree.max) / 2;
        Vector3 size = lTree.max - lTree.min;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

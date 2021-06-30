using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class BtTree : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btTree lTree = this.obj as btTree;
        Vector3 center = (lTree.worldMin + lTree.worldMax) / 2;
        Vector3 size = lTree.worldMax - lTree.worldMin;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

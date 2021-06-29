using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtStair : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btStair lStair = this.obj as btStair;
        Vector3 center = (lStair.worldMin + lStair.worldMax) / 2;
        Vector3 size = lStair.worldMax - lStair.worldMin;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

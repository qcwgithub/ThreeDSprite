using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtStair : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btStair lStair = this.obj as btStair;
        Vector3 center = (lStair.min + lStair.max) / 2;
        Vector3 size = lStair.max - lStair.min;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

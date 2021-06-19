using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtStair : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btStair lStair = this.obj as btStair;
        Vector3 min = LVector3.ToVector3(lStair.Data.Min);
        Vector3 max = LVector3.ToVector3(lStair.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

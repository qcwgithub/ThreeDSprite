using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtBoxObstacle : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btBoxObstacle bo = this.obj as btBoxObstacle;
        Vector3 min = LVector3.ToVector3(bo.Data.Min);
        Vector3 max = LVector3.ToVector3(bo.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtBoxObstacle : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btBoxObstacle bo = this.obj as btBoxObstacle;
        Vector3 center = (bo.worldMin + bo.worldMax) / 2;
        Vector3 size = bo.worldMax - bo.worldMin;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

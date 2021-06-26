using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtBoxObstacle : BtObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        btBoxObstacle bo = this.obj as btBoxObstacle;
        Vector3 center = (bo.min + bo.max) / 2;
        Vector3 size = bo.max - bo.min;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBoxObstacle : CObject
{
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        LBoxObstacle lBO = this.lObj as LBoxObstacle;
        Vector3 min = LVector3.ToVector3(lBO.Data.Min);
        Vector3 max = LVector3.ToVector3(lBO.Data.Max);
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

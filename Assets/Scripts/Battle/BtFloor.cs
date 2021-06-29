using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtFloor : BtObject
{
    protected btFloor floor;
    public override void Apply(BtScene scene, btObject obj)
    {
        base.Apply(scene, obj);
        this.floor = obj as btFloor;
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        Vector3 center = (floor.worldMin + floor.worldMax) / 2;
        Vector3 size = floor.worldMax - floor.worldMin;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

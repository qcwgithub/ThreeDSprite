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
        Vector3 center = (floor.min + floor.max) / 2;
        Vector3 size = floor.max - floor.min;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

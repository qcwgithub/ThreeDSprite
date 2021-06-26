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
// #if UNITY_EDITOR
//     protected override void OnDrawGizmosImpl()
//     {
//         Vector3 min = FVector3.ToVector3(this.floor.Data.min);
//         Vector3 max = FVector3.ToVector3(this.floor.Data.max);
//         Vector3 center = (min + max) / 2;
//         Vector3 size = max - min;
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireCube(center, size);
//     }
// #endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class BtWall: BtObject
{
    protected btWall wall;
    public override void Apply(BtBattle scene, btObject obj)
    {
        base.Apply(scene, obj);
        this.wall = obj as btWall;
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmosImpl()
    {
        Vector3 center = (wall.worldMin + wall.worldMax) / 2;
        Vector3 size = wall.worldMax - wall.worldMin;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObstacle : CObject
{
    [System.NonSerialized]
    public Vector3 Min;
    [System.NonSerialized]
    public Vector3 Max;

    public override void Apply()
    {
        base.Apply();

        Collider collider = this.GetComponent<Collider>();
        Bounds bounds = collider.bounds;
        this.Min = bounds.min;
        this.Max = bounds.max;
    }
    public virtual Vector3 LimitPos(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;

        // limit by bound
        if (to.x < this.Min.x) to.x = this.Min.x;
        else if (to.x > this.Max.x) to.x = this.Max.x;

        if (to.z < this.Min.z) to.z = this.Min.z;
        else if (to.z > this.Max.z) to.z = this.Max.z;

        return to;
    }

    public override void ObjectEnter(CObject obj)
    {
        CCharacter character = obj as CCharacter;
        if (character == null)
        {
            return;
        }
        character.ListObstacles.Add(this);
    }

    public override void ObjectExit(CObject obj)
    {
        CCharacter character = obj as CCharacter;
        if (character == null)
        {
            return;
        }
        character.ListObstacles.Add(this);
    }
}

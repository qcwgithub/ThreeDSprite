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
    public virtual bool LimitMove(Vector3 from, ref Vector3 delta)
    {
        Vector3 to = from + delta;
        if (to.x < this.Min.x || to.x > this.Max.x || to.z < this.Min.z || to.z > this.Max.z)
        {
            return false;
        }

        bool ret = false;
        if (from.x <= this.Min.x && delta.x > 0 && to.x > this.Min.x)
        {
            delta.x = 0;
            ret = true;
        }
        else if (from.x >= this.Max.x && delta.x < 0 && to.x < this.Max.x)
        {
            delta.x = 0;
            ret = true;
        }
        if (from.z <= this.Min.z && delta.z > 0 && to.z > this.Min.z)
        {
            delta.z = 0;
            ret = true;
        }
        else if (from.z >= this.Max.z && delta.z < 0 && to.z < this.Max.z)
        {
            delta.z = 0;
            ret = true;
        }
        return ret;
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

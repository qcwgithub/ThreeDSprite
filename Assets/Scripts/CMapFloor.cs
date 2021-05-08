using System;
using System.Collections.Generic;
using UnityEngine;

public class CMapFloor : CWalkable
{
    [NonSerialized]
    public float Y;

    [NonSerialized]
    public Vector3 Min;
    [NonSerialized]
    public Vector3 Max;
    public override void Init()
    {
        base.Init();
        this.Y = this.transform.position.y;
        this.Min.y = Y;
        this.Max.y = Y;

        BoxCollider[] colliders = this.GetComponentsInChildren<BoxCollider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (min.x < this.Min.x) this.Min.x = min.x;
            if (min.z < this.Min.z) this.Min.z = min.z;
            if (max.x > this.Max.x) this.Max.x = max.x;
            if (max.z > this.Max.z) this.Max.z = max.z;
        }
    }


    public override Vector3 RandomPos()
    {
        return new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), this.Y, UnityEngine.Random.Range(this.Min.z, this.Max.z));
    }

    public override Vector3 Move(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (to.x < this.Min.x) to.x = this.Min.x;
        else if (to.x > this.Max.x) to.x = this.Max.x;

        if (to.z < this.Min.z) to.z = this.Min.z;
        else if (to.z > this.Max.z) to.z = this.Max.z;

        to.y = this.Y;

        return to;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class CMapStairZ : CWalkable
{
    public BoxCollider collider1;
    public BoxCollider collider2;

    [NonSerialized]
    public Vector3 Min;
    [NonSerialized]
    public Vector3 Max;

    public override void Init()
    {
        base.Init();
        BoxCollider[] colliders = new BoxCollider[] { this.collider1, this.collider2 };

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (min.x < this.Min.x) this.Min.x = min.x;
            if (min.y < this.Min.y) this.Min.y = min.y;
            if (min.z < this.Min.z) this.Min.z = min.z;

            if (max.x > this.Max.x) this.Max.x = max.x;
            if (max.y > this.Max.y) this.Max.y = max.y;
            if (max.z > this.Max.z) this.Max.z = max.z;
        }

        Debug.Log("Stair " + this.Min + " ---- " + this.Max);
    }

    private float ZtoY(float z)
    {
        float t = (z - this.Min.z) / (this.Max.z - this.Min.z);
        float y = UnityEngine.Mathf.Lerp(this.Min.y, this.Max.y, t);
        return y;
    }

    public override Vector3 RandomPos()
    {
        Vector3 pos =  new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), 
            0f, 
            UnityEngine.Random.Range(this.Min.z, this.Max.z));

        pos.y = this.ZtoY(pos.z);
        return pos;
    }

    public override Vector3 Move(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (to.x < this.Min.x) to.x = this.Min.x;
        else if (to.x > this.Max.x) to.x = this.Max.x;

        if (to.z < this.Min.z) to.z = this.Min.z;
        else if (to.z > this.Max.z) to.z = this.Max.z;

        to.y = this.ZtoY(to.z);

        return to;
    }
}

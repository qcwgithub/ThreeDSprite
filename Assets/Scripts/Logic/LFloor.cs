using System;
using System.Collections.Generic;
using UnityEngine;

public class LFloor : LObject, IWalkable
{
    public BoxCollider collider1;
    public BoxCollider collider2;
    protected Vector3 Min;
    protected Vector3 Max;

    [NonSerialized]
    public float Y;

    //public override int Priority { get { return 1; } }

    public override void Apply()
    {
        base.Apply();
        this.Y = this.transform.position.y;
        this.Min.y = Y;
        this.Max.y = Y;

        BoxCollider[] colliders = new BoxCollider[] { this.collider1, this.collider2 };

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (i == 0 || min.x < this.Min.x) this.Min.x = min.x;
            if (i == 0 || min.z < this.Min.z) this.Min.z = min.z;
            if (i == 0 || max.x > this.Max.x) this.Max.x = max.x;
            if (i == 0 || max.z > this.Max.z) this.Max.z = max.z;
        }
    }

    protected bool CheckXZOutOfRange(Vector3 pos)
    {
        bool outOfRange = false;
        if (pos.x < this.Min.x)
        {
            pos.x = this.Min.x;
            outOfRange = true;
        }
        else if (pos.x > this.Max.x)
        {
            pos.x = this.Max.x;
            outOfRange = true;
        }

        if (pos.z < this.Min.z)
        {
            pos.z = this.Min.z;
            outOfRange = true;
        }
        else if (pos.z > this.Max.z)
        {
            pos.z = this.Max.z;
            outOfRange = true;
        }
        return outOfRange;
    }

    public Vector3 RandomPos()
    {
        return new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), this.Y, UnityEngine.Random.Range(this.Min.z, this.Max.z));
    }

    public PredictMoveResult PredictMove(Vector3 from, Vector3 delta)
    {
        PredictMoveResult result = default;
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            result.OutOfRange = true;
            return result;
        }

        result.Y = this.Y;
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }
        if (delta.y < 0 && from.y > this.Y && to.y <= this.Y)
        {
            return true;
        }
        if (Mathf.Abs(this.Y - to.y) > 0.1f)
        {
            return false;
        }
        return true;
    }
}

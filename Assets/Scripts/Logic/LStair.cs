using System;
using System.Collections.Generic;
using UnityEngine;

public enum StairDir
{
    Front_Back,
    LeftHigh_RightLow,
    LeftLow_RightHigh,
}
public class LStair : LObject, IWalkable
{
    public StairDir Dir;
    public BoxCollider collider1;
    public BoxCollider collider2;

    protected Vector3 Min;
    protected Vector3 Max;

    public override void Apply()
    {
        base.Apply();

        BoxCollider[] colliders = new BoxCollider[] { this.collider1, this.collider2 };

        for (int i = 0; i < colliders.Length; i++)
        {
            Bounds bound = colliders[i].bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            if (i == 0 || min.x < this.Min.x) this.Min.x = min.x;
            if (i == 0 || min.y < this.Min.y) this.Min.y = min.y;
            if (i == 0 || min.z < this.Min.z) this.Min.z = min.z;

            if (i == 0 || max.x > this.Max.x) this.Max.x = max.x;
            if (i == 0 || max.y > this.Max.y) this.Max.y = max.y;
            if (i == 0 || max.z > this.Max.z) this.Max.z = max.z;
        }

        Debug.Log("Stair" + this.Id + ": Min" + this.Min + ", Max" + this.Max);
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

    private float ZtoY(float z)
    {
        float t = (z - this.Min.z) / (this.Max.z - this.Min.z);
        float y = UnityEngine.Mathf.Lerp(this.Min.y, this.Max.y, t);
        return y;
    }

    private float XtoY(float x, StairDir dir)
    {
        if (dir == StairDir.LeftLow_RightHigh)
        {
            float t = (x - this.Min.x) / (this.Max.x - this.Min.x);
            float y = UnityEngine.Mathf.Lerp(this.Min.y, this.Max.y, t);
            return y;
        }
        else
        {
            float t = (x - this.Min.x) / (this.Max.x - this.Min.x);
            float y = UnityEngine.Mathf.Lerp(this.Max.y, this.Min.y, t);
            return y;
        }
    }

    private float XZtoY(float x, float z)
    {
        switch (this.Dir)
        {
            case StairDir.Front_Back:
                return this.ZtoY(z);
                //break;
            case StairDir.LeftHigh_RightLow:
            case StairDir.LeftLow_RightHigh:
            default:
                return this.XtoY(x, this.Dir);
                //break;
        }
    }

    public Vector3 RandomPos()
    {
        Vector3 pos =  new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), 
            0f, 
            UnityEngine.Random.Range(this.Min.z, this.Max.z));

        pos.y = this.ZtoY(pos.z);
        return pos;
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

        result.Y = this.XZtoY(to.x, to.z);
        return result;
    }

    public bool CanAccept(Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        if (this.CheckXZOutOfRange(to))
        {
            return false;
        }

        float y = this.XZtoY(to.x, to.z);
        if (delta.y < 0 && from.y > y && to.y <= y)
        {
            return true;
        }
        if (Mathf.Abs(to.y - y) > 0.1f)
        {
            return false;
        }
        return true;
    }
}

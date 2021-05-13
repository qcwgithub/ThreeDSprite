using System;
using System.Collections.Generic;
using UnityEngine;

public enum StairDir
{
    Front_Back,
    LeftHigh_RightLow,
    LeftLow_RightHigh,
}
public class CMapStair : CWalkable
{
    public StairDir Dir;
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

            if (i == 0 || min.x < this.Min.x) this.Min.x = min.x;
            if (i == 0 || min.y < this.Min.y) this.Min.y = min.y;
            if (i == 0 || min.z < this.Min.z) this.Min.z = min.z;

            if (i == 0 || max.x > this.Max.x) this.Max.x = max.x;
            if (i == 0 || max.y > this.Max.y) this.Max.y = max.y;
            if (i == 0 || max.z > this.Max.z) this.Max.z = max.z;
        }

        Debug.Log("Stair" + this.Id + ": Min" + this.Min + ", Max" + this.Max);
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

    public override Vector3 RandomPos()
    {
        Vector3 pos =  new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), 
            0f, 
            UnityEngine.Random.Range(this.Min.z, this.Max.z));

        pos.y = this.ZtoY(pos.z);
        return pos;
    }

    public override void Move(CCharacter character, Vector3 delta)
    {
        Vector3 to = character.Pos + delta;
        if (to.x < this.Min.x) to.x = this.Min.x;
        else if (to.x > this.Max.x) to.x = this.Max.x;

        if (to.z < this.Min.z) to.z = this.Min.z;
        else if (to.z > this.Max.z) to.z = this.Max.z;

        switch (this.Dir)
        {
            case StairDir.Front_Back:
                to.y = this.ZtoY(to.z);
                break;
            case StairDir.LeftHigh_RightLow:
            case StairDir.LeftLow_RightHigh:
                to.y = this.XtoY(to.x, this.Dir);
                break;
        }

        character.Pos = to;
    }

    public override bool isXZInRange(Vector3 pos)
    {
        return pos.x < this.Min.x ||
            pos.x > this.Max.x ||
            pos.z < this.Min.z ||
            pos.z > this.Max.z;
    }
}

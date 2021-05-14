using System;
using System.Collections.Generic;
using UnityEngine;

public class CFloor : CWalkable
{
    public BoxCollider collider1;
    public BoxCollider collider2;

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


    public override Vector3 RandomPos()
    {
        return new Vector3(UnityEngine.Random.Range(this.Min.x, this.Max.x), this.Y, UnityEngine.Random.Range(this.Min.z, this.Max.z));
    }

    public override void Move(CCharacter character, Vector3 delta)
    {
        Vector3 to = character.Pos + delta;
        to = this.LimitPos(character.ListObstacles, character.Pos, delta);

        to.y = this.Y;

        character.Pos = to;
    }

    public override bool IsXZInRange(Vector3 pos)
    {
        return pos.x < this.Min.x ||
            pos.x > this.Max.x ||
            pos.z < this.Min.z ||
            pos.z > this.Max.z;
    }
}

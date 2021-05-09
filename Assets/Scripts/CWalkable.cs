using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CWalkable : MonoBehaviour
{
    public int Id;
    public Collider BoundingCollider;

    public virtual int Priority { get { return 0; } }
    public virtual void Init()
    {

    }

    public virtual Vector3 RandomPos()
    {
        return Vector3.zero;
    }

    public virtual Vector3 Move(Vector3 from, Vector3 delta)
    {
        return from;
    }
}
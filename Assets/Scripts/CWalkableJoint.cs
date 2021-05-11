using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WalkableJointSides
{
    Front_Back,
    Left_Right,
}

public class CWalkableJoint : MonoBehaviour
{
    public WalkableJointSides Sides;
    public CWalkable Walkable1;
    public CWalkable Walkable2;

    public BoxCollider Collider { get; private set; }

    private void Awake()
    {
        this.Collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }
}

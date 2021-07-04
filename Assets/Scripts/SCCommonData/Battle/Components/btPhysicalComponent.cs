using System.Collections;
using System.Collections.Generic;
using Data;
using System;
using UnityEngine;

public struct btPhysicalComponent
{
    public q3BodyType bodyType;
    public IntPtr body;
    // public IntPtr box;
    public Vector3 worldMin;
    public Vector3 worldMax;
}

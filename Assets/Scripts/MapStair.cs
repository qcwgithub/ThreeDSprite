using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StairDir
{
    X,
    Z,
}

public class MapStair : MonoBehaviour
{
    public StairDir Dir = StairDir.X;
    public MapRoom Lower;
    public MapRoom Higher;
}

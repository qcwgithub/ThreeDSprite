using System;
using UnityEngine;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    private static Bootstrap instance = null;
    public static Bootstrap Instance { get { return instance; } }

    protected void Awake()
    {
        Bootstrap.instance = this;
    }

    protected void Start() 
    {
    }

    public void Init()
    {

    }

    protected void Update()
    {
    }


    public void logout()
    {

    }
}
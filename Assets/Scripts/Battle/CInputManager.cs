using System;
using System.Collections.Generic;
using UnityEngine;

public class CInputManager : MonoBehaviour
{
    public event Action<Vector3> OnInput;

    void Update()
    {
        if (this.OnInput != null)
        {
            Vector3 dir = Vector3.zero;
            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");
            //if (Character.Walkable == null)
            //{
            //    dir.y = -1f;
            //}
            this.OnInput(dir);
        }
    }
}

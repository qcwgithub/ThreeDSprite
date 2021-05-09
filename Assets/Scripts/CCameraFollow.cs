using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// http://gyanendushekhar.com/2020/03/10/smooth-camera-follow-in-unity-3d/
public class CCameraFollow : MonoBehaviour
{
    public Transform Target;
    public bool Pause = false;
    // change this value to get desired smoothness
    public float SmoothTime = 0.3f;
    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;

    private Vector3 offset;
    private void Start()
    {
        this.offset = this.transform.position - this.Target.position;
    }

    private void LateUpdate()
    {
        if (this.Target == null || this.Pause)
        {
            return;
        }

        Vector3 toPos = this.Target.position + this.offset;
        Vector3 position = Vector3.SmoothDamp(transform.position, toPos, ref velocity, SmoothTime);
        transform.position = position;
    }
}

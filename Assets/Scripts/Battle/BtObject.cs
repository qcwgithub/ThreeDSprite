using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class BtObject : MonoBehaviour
{
    public int Id;
    public btObject obj { get; protected set; }
    public BtScene scene { get; protected set; }
    public virtual void Apply(BtScene scene, btObject obj)
    {
        this.scene = scene;
        this.obj = obj;
    }
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (this.scene == null) return;
        if (!this.scene.DrawGizmos_ObjectBounds)
        {
            return;
        }
        this.OnDrawGizmosImpl();
    }
    protected virtual void OnDrawGizmosImpl()
    {

    }
#endif
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class CCharacter : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    private Transform trans;
    private Vector3 originalScale;
    public void Apply(LCharacter lChar)
    {
        this.trans = this.transform;
        this.originalScale = this.trans.localScale;
        lChar.PosChanged += this.OnPosChanged;
    }

    private bool moved = false;
    private float movedX = 0f;
    private void OnPosChanged(Vector3 pos)
    {
        this.movedX = pos.x - this.trans.position.x;
        this.trans.position = pos;
        this.moved = true;
    }
    private void Update()
    {
        if (this.moved)
        {
            this.Skel.AnimationName = "run";
        }
        else
        {
            this.Skel.AnimationName = "idle";
        }
        this.Skel.loop = true;
        this.trans.localScale = new Vector3(this.movedX < 0 ? -1f : 1f, this.originalScale.y, this.originalScale.z);
        this.moved = false;
    }
}

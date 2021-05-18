using System;
using System.Collections.Generic;
using UnityEngine;

public class CCharacter : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    private Transform trans;
    public void Apply(LCharacter lChar)
    {
        this.trans = this.transform;
        lChar.PosChanged += this.OnPosChanged;
    }

    private void OnPosChanged(Vector3 pos)
    {
        this.trans.position = pos;
    }
}

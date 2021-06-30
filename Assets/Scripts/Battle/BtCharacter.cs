using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Data;

public class BtCharacter : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    private Transform trans;
    private Vector3 originalScale;
    public btCharacter lChar { get; private set; }
    private BtScene cMap;
    public void Apply(btCharacter character, BtScene scene)
    {
        this.trans = this.transform;
        this.originalScale = this.trans.localScale;
        this.lChar = character;
        character.PosChanged += this.OnPosChanged;
        this.cMap = scene;
    }

    private bool moved = false;
    private float movedX = 0f;
    private void OnPosChanged(Vector3 pos)
    {
        this.movedX = pos.x - this.trans.position.x;
        this.trans.position = pos;
        this.moved = true;
    }

    private List<btObject> colliding1 = new List<btObject>();
    private List<btObject> colliding2 = new List<btObject>();
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

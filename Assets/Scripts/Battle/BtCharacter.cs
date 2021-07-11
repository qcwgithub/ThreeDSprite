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
    public btCharacter character { get; private set; }
    private BtScene cMap;
    public void Apply(btCharacter character, BtScene scene)
    {
        this.trans = this.transform;
        this.originalScale = this.trans.localScale;
        this.character = character;
        // character.PosChanged += this.OnPosChanged;
        this.cMap = scene;
        this.trans.position = this.character.pos;
    }
#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        if (character == null) return;
        var min = character.worldMin + character.pos;
        var max = character.worldMax + character.pos;

        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif

    // private bool moved = false;
    // private float movedX = 0f;
    // private void OnPosChanged(Vector3 pos)
    // {
    //     this.movedX = pos.x - this.trans.position.x;
    //     this.trans.position = pos;
    //     this.moved = true;
    // }

    private bool wasMoving = false;
    private void Update()
    {
        bool moving = this.character.moveDir != Vector3.zero;
        if (moving)
        {
            this.trans.position = this.character.pos;
            this.trans.localScale = new Vector3(this.character.moveDir.x < 0 ? -1f : 1f, this.originalScale.y, this.originalScale.z);
        }
        if (moving != this.wasMoving)
        {
            this.Skel.AnimationName = moving ? "run" : "idle";
            this.Skel.loop = true;
            this.wasMoving = moving;
        }
    }
}

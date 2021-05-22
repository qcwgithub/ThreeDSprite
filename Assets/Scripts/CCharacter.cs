using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CCharacter : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    private Transform trans;
    private Vector3 originalScale;
    public LCharacter lChar { get; private set; }
    private CMap cMap;
    public void Apply(LCharacter lChar, CMap map)
    {
        this.trans = this.transform;
        this.originalScale = this.trans.localScale;
        this.lChar = lChar;
        lChar.PosChanged += this.OnPosChanged;
        this.cMap = map;
    }

    private bool moved = false;
    private float movedX = 0f;
    private void OnPosChanged(Vector3 pos)
    {
        this.movedX = pos.x - this.trans.position.x;
        this.trans.position = pos;
        this.moved = true;
    }

    private List<LObject> colliding1 = new List<LObject>();
    private List<LObject> colliding2 = new List<LObject>();
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

        // 
        Bounds bounds = new Bounds(this.lChar.Pos, new Vector3(0.4f, 0.4f, 0.4f));
        this.colliding1.Clear();
        this.cMap.lMap.Octree.GetColliding(this.colliding1, bounds);

        bool different = this.colliding1.Count != this.colliding2.Count;
        if (!different)
        {
            for (int i = 0; i < this.colliding1.Count; i++)
            {
                if (this.colliding1[i].Id != this.colliding2[i].Id)
                {
                    different = true;
                    break;
                }
            }
        }
        if (different)
        {
            var sb = new StringBuilder();
            sb.Append("Collides with: ");
            for (int i = 0; i < this.colliding1.Count; i++)
            {
                sb.AppendFormat("{0}{1}", this.colliding1[i].Type, this.colliding1[i].Id);
                if (i < this.colliding1.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            Debug.Log(sb.ToString());
            //
            this.colliding2.Clear();
            this.colliding2.AddRange(this.colliding1);
        }

    }
}

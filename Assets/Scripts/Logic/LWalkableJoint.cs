using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WalkableJointSides
{
    Front_Back,
    Left_Right,
}

// workflow
// 1 �������ײ�壬��ʼ���λ�ñ仯���뿪��ֹͣ���
// 2 �Խ�ɫ�������ߵ�ǰ���Ǻ��л������� Walkable
public class LWalkableJoint : MonoBehaviour// LWalkable
{
    public WalkableJointSides Sides;
    //public LWalkable Walkable1;
   //public LWalkable Walkable2;

    //public BoxCollider Collider { get; private set; }
    //private Bounds ColliderBounds;

    //public override void Apply()
    //{
    //    base.Apply();

    //    this.Collider = GetComponent<BoxCollider>();
    //    this.ColliderBounds = this.Collider.bounds;
    //    this.Min = this.ColliderBounds.min;
    //    this.Max = this.ColliderBounds.max;
    //}
    //private LWalkable CalcWalkable(Vector3 pos)
    //{
    //    Vector3 thisPos = this.transform.position;
    //    switch (this.Sides)
    //    {
    //        case WalkableJointSides.Front_Back:
    //            return (pos.z > thisPos.z) ? this.Walkable1 : this.Walkable2;

    //        case WalkableJointSides.Left_Right:
    //        default:
    //            return (pos.x < thisPos.x) ? this.Walkable1 : this.Walkable2;
    //    }
    //}

    //public override void ObjectEnter(LObject obj)
    //{
    //    CCharacter character = obj as CCharacter;
    //    if (character == null)
    //    {
    //        return;
    //    }
    //    Debug.Log(string.Format("{0} CharacterEnter", this.name));
    //    character.Walkable = this;
    //}
    //public override void ObjectExit(LObject obj)
    //{
    //    CCharacter character = obj as CCharacter;
    //    if (character == null)
    //    {
    //        return;
    //    }
    //    Debug.Log(string.Format("{0} CharacterExit", this.name));
    //    character.Walkable = this.CalcWalkable(character.Pos);
    //}

    //public override Vector3 RandomPos()
    //{
    //    Vector3 thisPos = this.transform.position;
    //    return thisPos;
    //}

    //public override void Move(CCharacter character, Vector3 delta)
    //{
    //    Vector3 to = character.Pos + delta;
    //    LWalkable walkable = this.CalcWalkable(to);
    //    walkable.Move(character, delta);
    //}

    //public override bool IsXZInRange(Vector3 pos)
    //{
    //    return pos.x < this.Min.x ||
    //        pos.x > this.Max.x ||
    //        pos.z < this.Min.z ||
    //        pos.z > this.Max.z;
    //}
}

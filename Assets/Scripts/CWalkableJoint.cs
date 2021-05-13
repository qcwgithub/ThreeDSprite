using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WalkableJointSides
{
    Front_Back,
    Left_Right,
}

// workflow
// 1 进入此碰撞体，开始监测位置变化。离开后，停止监测
// 2 以角色处于中线的前还是后，切换所属的 Walkable
// 注意：
// 1 要配置正确的中线位置
public class CWalkableJoint : CWalkable
{
    public WalkableJointSides Sides;
    public CWalkable Walkable1;
    public CWalkable Walkable2;

    public BoxCollider Collider { get; private set; }
    private Bounds ColliderBounds;
    private Vector3 Min;
    private Vector3 Max;

    private void Awake()
    {
        this.Collider = GetComponent<BoxCollider>();
        this.ColliderBounds = this.Collider.bounds;
        this.Min = this.ColliderBounds.min;
        this.Max = this.ColliderBounds.max;
    }
    private CWalkable CalcWalkable(Vector3 pos)
    {
        Vector3 thisPos = this.transform.position;
        switch (this.Sides)
        {
            case WalkableJointSides.Front_Back:
                return (pos.z > thisPos.z) ? this.Walkable1 : this.Walkable2;

            case WalkableJointSides.Left_Right:
            default:
                return (pos.x < thisPos.x) ? this.Walkable1 : this.Walkable2;
        }
    }

    public void CharacterEnter(CCharacter character)
    {
        Debug.Log(string.Format("{0} CharacterEnter", this.name));
        character.Walkable = this;
    }
    public void CharacterExit(CCharacter character)
    {
        Debug.Log(string.Format("{0} CharacterExit", this.name));
        character.Walkable = this.CalcWalkable(character.Pos);
    }

    public override Vector3 RandomPos()
    {
        Vector3 thisPos = this.transform.position;
        return thisPos;
    }

    public override void Move(CCharacter character, Vector3 delta)
    {
        Vector3 to = character.Pos + delta;
        CWalkable walkable = this.CalcWalkable(to);
        walkable.Move(character, delta);
    }

    public override bool isXZInRange(Vector3 pos)
    {
        return pos.x < this.Min.x ||
            pos.x > this.Max.x ||
            pos.z < this.Min.z ||
            pos.z > this.Max.z;
    }
}

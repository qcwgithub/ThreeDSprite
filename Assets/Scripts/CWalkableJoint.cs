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
public class CWalkableJoint : MonoBehaviour
{
    public WalkableJointSides Sides;
    public CWalkable Walkable1;
    public CWalkable Walkable2;

    public BoxCollider Collider { get; private set; }

    private void Awake()
    {
        this.Collider = GetComponent<BoxCollider>();
    }
    private void SelectWalkable(CCharacter char_)
    {
        Vector3 thisPos = this.transform.position;
        Vector3 charPos = char_.Pos;
        switch (this.Sides)
        {
            case WalkableJointSides.Front_Back:
                char_.CurrWalkable = (charPos.z > thisPos.z) ? this.Walkable1 : this.Walkable2;
                break;
            case WalkableJointSides.Left_Right:
                char_.CurrWalkable = (charPos.x < thisPos.x) ? this.Walkable1 : this.Walkable2;
                break;
        }
    }
    public void CharacterEnter(CCharacter char_)
    {
        Debug.Log(string.Format("{0} CharacterEnter", this.name));
        this.SelectWalkable(char_);
        char_.PosChanged += this.OnCharacterPosChanged;
    }
    public void CharacterExit(CCharacter char_)
    {
        Debug.Log(string.Format("{0} CharacterExit", this.name));
        char_.PosChanged -= this.OnCharacterPosChanged;
    }
    private void OnCharacterPosChanged(CCharacter char_)
    {
        this.SelectWalkable(char_);
    }
}

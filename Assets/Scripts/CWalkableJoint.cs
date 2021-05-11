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
// ע�⣺
// 1 Ҫ������ȷ������λ��
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

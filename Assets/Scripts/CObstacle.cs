using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObstacle : CObject
{
    public virtual Vector3 LimitPos(Vector3 to)
    {
        return to;
    }

    public override void ObjectEnter(CObject obj)
    {
        CCharacter character = obj as CCharacter;
        if (character == null)
        {
            return;
        }
        character.ListObstacles.Add(this);
    }

    public override void ObjectExit(CObject obj)
    {
        CCharacter character = obj as CCharacter;
        if (character == null)
        {
            return;
        }
        character.ListObstacles.Remove(this);
    }
}

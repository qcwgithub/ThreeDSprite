using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMap : MonoBehaviour
{
    public List<CWalkable> Walkables;
    //public Dictionary<Collider, CWalkable> DictWalkables;
    //public Dictionary<Collider, CWalkableJoint> DictJoints;

    public virtual void Init()
    {
        for (int i = 0; i < this.Walkables.Count; i++)
        {
            this.Walkables[i].Init();
        }

        //this.DictWalkables = new Dictionary<Collider, CWalkable>();
        //for (int i = 0; i < this.Walkables.Count; i++)
        //{
        //    this.DictWalkables.Add(this.Walkables[i].BoundingCollider, this.Walkables[i]);
        //}

        //this.DictJoints = new Dictionary<Collider, CWalkableJoint>();
        //CWalkableJoint[] joints = this.GetComponentsInChildren<CWalkableJoint>();
        //for (int i = 0; i < joints.Length; i++)
        //{
        //    this.DictJoints.Add(joints[i].Collider, joints[i]);
        //}
    }

    public CWalkable RandomWalkable()
    {
        int index = Random.Range(0, this.Walkables.Count);
        return this.Walkables[index];
    }

    public void AddCharacter(CCharacter character)
    {
        character.Walkable = this.Walkables[0];// this.RandomWalkable
        //char_.transform.position = char_.CurrWalkable.RandomPos();
        character.InitPos();

        character._OnTriggerEnter += this._OnTriggerEnter;
        character._OnTriggerExit += this._OnTriggerExit;
    }

    //private void SelectWalkable(CCharacter char_)
    //{
    //    int count = char_.ListWalkables.Count;
    //    if (count == 0)
    //    {
    //        return;
    //    }
    //    if (count == 1)
    //    {
    //        char_.CurrWalkable = char_.ListWalkables[0];
    //        return;
    //    }

    //    CWalkable highest = null;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (highest == null || char_.ListWalkables[i].Priority > highest.Priority)
    //        {
    //            highest = char_.ListWalkables[i];
    //        }
    //    }

    //    // Change walkable
    //    char_.CurrWalkable = highest;

    //}

    private void _OnTriggerEnter(CCharacter character, Collider other)
    {
        //CWalkable walkable;
        //if (!this.DictWalkables.TryGetValue(other, out walkable))
        //{
        //    return;
        //}

        //if (char_.CurrWalkable == walkable || char_.ListWalkables.Contains(walkable))
        //{
        //    return;
        //}

        //char_.ListWalkables.Add(walkable);
        //this.SelectWalkable(char_);

        //CWalkableJoint joint;
        //if (!this.DictJoints.TryGetValue(other, out joint))
        //{
        //    return;
        //}

        CWalkableJoint joint = other.GetComponent<CWalkableJoint>();
        if (joint == null)
        {
            return;
        }

        Debug.Assert(joint == character.Walkable || joint.Walkable1 == character.Walkable || joint.Walkable2 == character.Walkable);
        joint.CharacterEnter(character);
    }

    private void _OnTriggerExit(CCharacter character, Collider other)
    {
        //CWalkable walkable;
        //if (!this.DictWalkables.TryGetValue(other, out walkable))
        //{
        //    return;
        //}

        //if (char_.CurrWalkable == walkable)
        //{
        //    char_.ListWalkables.Remove(char_.CurrWalkable);
        //    this.SelectWalkable(char_);
        //    return;
        //}

        //char_.ListWalkables.Remove(walkable);
        CWalkableJoint joint = other.GetComponent<CWalkableJoint>();
        if (joint == null)
        {
            return;
        }

        Debug.Assert(joint == character.Walkable || joint.Walkable1 == character.Walkable || joint.Walkable2 == character.Walkable);
        joint.CharacterExit(character);
    }
}

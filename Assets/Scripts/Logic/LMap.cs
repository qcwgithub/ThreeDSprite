using System.Collections.Generic;
using UnityEngine;

public class LMap : MonoBehaviour
{
    public Dictionary<int, LObject> DictObjects = new Dictionary<int, LObject>();
    public List<IWalkable> Walkables = new List<IWalkable>();
    public List<IObstacle> Obstacles = new List<IObstacle>();

    public virtual void Apply()
    {
        LObject[] objects = this.GetComponentsInChildren<LObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            LObject obj = objects[i];
            if (!obj.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (this.DictObjects.ContainsKey(obj.Id))
            {
                Debug.LogError("Object Id duplicated: " + obj.Id);
                continue;
            }
            this.DictObjects.Add(obj.Id, obj);

            if (obj is IWalkable)
            {
                this.Walkables.Add(obj as IWalkable);
            }
            else if (obj is IObstacle)
            {
                this.Obstacles.Add(obj as IObstacle);
            }
        }
        this.Walkables.Sort((a, b) => (a as LObject).Id - (b as LObject).Id);

        foreach (var kv in this.DictObjects)
        {
            kv.Value.Apply();
        }
    }

    public void Move(CCharacter character, Vector3 delta)
    {
        Vector3 from = character.Pos;
        float y = 0f;
        IWalkable preWalkable = character.Walkable;

        if (character.Walkable != null)
        {
            PredictMoveResult result = character.Walkable.PredictMove(from, delta);
            if (!result.OutOfRange)
            {
                //Debug.Log("OutOfRange")
                y = result.Y;
            }
            else
            {
                Debug.Log("Out of range, " + ((LObject)character.Walkable).Id);
                character.Walkable = null;
            }
        }

        if (character.Walkable == null)
        {
            for (int i = 0; i < this.Walkables.Count; i++)
            {
                if (this.Walkables[i] != preWalkable && this.Walkables[i].CanAccept(from, delta))
                {
                    character.Walkable = this.Walkables[i];
                    PredictMoveResult result = this.Walkables[i].PredictMove(from, delta);
                    y = result.Y;
                    break;
                }
            }
        }

        if (character.Walkable != null)
        {
            delta.y = y - from.y;
        }

        for (int i = 0; i < this.Obstacles.Count; i++)
        {
            if (this.Obstacles[i].LimitMove(from, ref delta))
            {
                break;
            }
        }

        character.Pos = from + delta;
    }

    public IWalkable RandomWalkable()
    {
        int index = Random.Range(0, this.Walkables.Count);
        return this.Walkables[index];
    }

    public void AddCharacter(CCharacter character)
    {
        //character.Walkable = this.Walkables[0];// this.RandomWalkable
        //char_.transform.position = char_.CurrWalkable.RandomPos;
        //character.Pos = character.transform.position;
        //character.Walkable.Move(character, Vector3.zero);

        //character._OnTriggerEnter += this._OnTriggerEnter;
        //character._OnTriggerExit += this._OnTriggerExit;
    }

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

        LObject obj = other.GetComponent<LObject>();
        if (obj == null)
        {
            return;
        }

        //Debug.Assert(joint == character.Walkable || joint.Walkable1 == character.Walkable || joint.Walkable2 == character.Walkable);
        //joint.CharacterEnter(character);
        obj.ObjectEnter(character);
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
        LObject obj = other.GetComponent<LObject>();
        if (obj == null)
        {
            return;
        }

        //Debug.Assert(joint == character.Walkable || joint.Walkable1 == character.Walkable || joint.Walkable2 == character.Walkable);
        obj.ObjectExit(character);
    }
}

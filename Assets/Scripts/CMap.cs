using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMap : MonoBehaviour
{
    public List<CWalkable> Walkables;
    public Dictionary<Collider, CWalkable> DictWalkables;

    public virtual void Init()
    {
        for (int i = 0; i < this.Walkables.Count; i++)
        {
            this.Walkables[i].Init();
        }

        this.DictWalkables = new Dictionary<Collider, CWalkable>();
        for (int i = 0; i < this.Walkables.Count; i++)
        {
            this.DictWalkables.Add(this.Walkables[i].BoundingCollider, this.Walkables[i]);
        }
    }

    public CWalkable RandomWalkable()
    {
        int index = Random.Range(0, this.Walkables.Count);
        return this.Walkables[index];
    }

    public void AddCharacter(CCharacter char_)
    {
        char_.CurrWalkable = this.RandomWalkable();
        char_.transform.position = char_.CurrWalkable.RandomPos();

        char_.ActionOnTriggerEnter += this._OnTriggerEnter;
        char_.ActionOnTriggerExit += this._OnTriggerExit;
    }

    private void SelectWalkable(CCharacter char_)
    {
        int count = char_.ListWalkables.Count;
        if (count == 0)
        {
            return;
        }
        if (count == 1)
        {
            char_.CurrWalkable = char_.ListWalkables[0];
            return;
        }

        CWalkable highest = null;
        for (int i = 0; i < count; i++)
        {
            if (highest == null || char_.ListWalkables[i].Priority > highest.Priority)
            {
                highest = char_.ListWalkables[i];
            }
        }

        // Change walkable
        char_.CurrWalkable = highest;

    }

    private void _OnTriggerEnter(CCharacter char_, Collider other)
    {
        CWalkable walkable;
        if (!this.DictWalkables.TryGetValue(other, out walkable))
        {
            return;
        }

        if (char_.CurrWalkable == walkable || char_.ListWalkables.Contains(walkable))
        {
            return;
        }

        char_.ListWalkables.Add(walkable);
        this.SelectWalkable(char_);
    }

    private void _OnTriggerExit(CCharacter char_, Collider other)
    {
        CWalkable walkable;
        if (!this.DictWalkables.TryGetValue(other, out walkable))
        {
            return;
        }

        if (char_.CurrWalkable == walkable)
        {
            char_.ListWalkables.Remove(char_.CurrWalkable);
            this.SelectWalkable(char_);
            return;
        }

        char_.ListWalkables.Remove(walkable);
    }
}

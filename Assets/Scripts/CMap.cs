using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMap : MonoBehaviour
{
    [System.Serializable]
    public class LinkInfo
    {
        public Collider Collider;
        public CWalkable EnterTo;
        public CWalkable ExitTo;
    }

    public List<CWalkable> Walkables;
    public List<LinkInfo> Links;

    public Dictionary<Collider, LinkInfo> DictLink;

    public virtual void Init()
    {
        for (int i = 0; i < this.Walkables.Count; i++)
        {
            this.Walkables[i].Init();
        }

        this.DictLink = new Dictionary<Collider, LinkInfo>();
        for (int i = 0; i < this.Links.Count; i++)
        {
            this.DictLink.Add(this.Links[i].Collider, this.Links[i]);
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

        char_.ActionOnTriggerEnter += this.ActionOnCollisionEnter;
        char_.ActionOnTriggerExit += this.ActionOnCollisionExit;
    }
    private void ActionOnCollisionEnter(CCharacter char_, Collider other)
    {
        LinkInfo link;
        if (!this.DictLink.TryGetValue(other, out link))
        {
            return;
        }

        // Change walkable
        char_.CurrWalkable = link.EnterTo;
    }

    private void ActionOnCollisionExit(CCharacter char_, Collider other)
    {
        LinkInfo link;
        if (!this.DictLink.TryGetValue(other, out link))
        {
            return;
        }

        // Change walkable
        char_.CurrWalkable = link.ExitTo;
    }
}

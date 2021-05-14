using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CWalkable : CObject
{
    protected Vector3 Min;
    protected Vector3 Max;
    //protected List<CObstacle> ListObstacles;
    //public virtual int Priority { get { return 0; } }

    public override void Apply()
    {
        base.Apply();

        //CObstacle[] obstacles = this.GetComponentsInChildren<CObstacle>();
        //if (obstacles.Length > 0)
        //{
        //    this.ListObstacles = new List<CObstacle>();
        //    this.ListObstacles.AddRange(obstacles);
        //}
    }

    public virtual Vector3 RandomPos()
    {
        return Vector3.zero;
    }

    protected virtual Vector3 LimitPos(List<CObstacle> obstacles, Vector3 from, Vector3 delta)
    {
        Vector3 to = from + delta;
        for (int i = 0; i < obstacles.Count; i++)
        {
            to = obstacles[i].LimitPos(from, delta);
        }

        // limit by bound
        if (to.x < this.Min.x) to.x = this.Min.x;
        else if (to.x > this.Max.x) to.x = this.Max.x;

        if (to.z < this.Min.z) to.z = this.Min.z;
        else if (to.z > this.Max.z) to.z = this.Max.z;

        return to;
    }
    public virtual void Move(CCharacter character, Vector3 delta)
    {

    }

    public virtual bool IsXZInRange(Vector3 pos)
    {
        return false;
    }
}

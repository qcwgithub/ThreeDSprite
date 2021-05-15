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

    protected virtual bool LimitPos(List<CObstacle> obstacles, Vector3 from, ref Vector3 delta)
    {
        bool ret = false;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].LimitMove(from, ref delta))
            {
                ret = true;
                break;
            }
        }
        Vector3 to = from + delta;
        if (to.x >= this.Min.x && to.x <= this.Max.x && to.z >= this.Min.z && to.z <= this.Max.z)
        {
            return ret;
        }

        // limit by bound
        if (to.x < this.Min.x)
        {
            delta.x = this.Min.x - from.x;
            ret = true;
        }
        else if (to.x > this.Max.x)
        {
            delta.x = this.Max.x - from.x;
            ret = true;
        }

        if (to.z < this.Min.z)
        {
            delta.z = this.Min.z - from.z;
            ret = true;
        }
        else if (to.z > this.Max.z)
        {
            delta.z = this.Max.z - from.z;
            ret = true;
        }

        return ret;
    }
    public virtual void Move(CCharacter character, Vector3 delta)
    {

    }

    public virtual bool IsXZInRange(Vector3 pos)
    {
        return false;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Data
{
    public struct btObject_Time
    {
        public btObject obj;
        public float time;
    }


    public class btCharacter : btObject, btIHasPosition
    {
        public event Action<Vector3> PosChanged;

        public btIWalkable walkable;
        // public bool EverHasWalkable { get; private set; }
        // public btIWalkable Walkable
        // {
        //     get { return this.walkable; }
        //     set
        //     {
        //         btIWalkable pre = this.walkable;
        //         if (pre == value)
        //         {
        //             return;
        //         }
        //         this.walkable = value;

        //         if (value != null)
        //         {
        //             this.EverHasWalkable = true;
        //         }

        //         Debug.Log(string.Format("{0} -> {1}",
        //             pre == null ? "null" : pre.ToString(),
        //             value == null ? "null" : value.ToString()));
        //     }
        // }

        // 需要记住每帧移动前的位置，之后碰到障碍时才可以退回去
        public Vector3 prePos { get; set; }
        public Vector3 pos { get; set; }
        // public Vector3 Pos
        // {
        //     get
        //     {
        //         return this.pos;
        //     }
        //     set
        //     {
        //         this.pos = value;

        //         if (this.body != IntPtr.Zero)
        //             battle.SetBodyPosition(this.body, value);

        //         if (this.PosChanged != null)
        //         {
        //             this.PosChanged(value);
        //         }
        //     }
        // }

        public Vector3 moveDir;
    }
}
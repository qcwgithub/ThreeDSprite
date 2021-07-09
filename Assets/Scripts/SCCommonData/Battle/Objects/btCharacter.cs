using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Data
{
    public class btCharacter : btObject, btIHasPosition
    {
        // public event Action<Vector3> PosChanged;
        public int playerId;

        public btIWalkable walkable;

        // 需要记住每帧移动前的位置，之后碰到障碍时才可以退回去
        public Vector3 prePos { get; set; }
        public Vector3 pos { get; set; }

        public Vector3 moveDir;
    }
}
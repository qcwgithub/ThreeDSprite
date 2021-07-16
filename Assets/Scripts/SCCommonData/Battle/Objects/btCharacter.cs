using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class btCharacter : btObject, btIHasPosition
    {
        // public event Action<Vector3> PosChanged;

        [Key(2)]
        public int playerId;

        [Key(3)]
        public int walkableId;

        [IgnoreMember]
        private btIWalkable _walkable;

        [IgnoreMember]
        public btIWalkable walkable
        {
            get
            {
                return _walkable;
            }
            set
            {
                _walkable = value;
                this.walkableId = value != null ? ((btObject)value).id : 0;
            }
        }

        // 需要记住每帧移动前的位置，之后碰到障碍时才可以退回去
        [IgnoreMember]
        public Vector3 prePos { get; set; }

        [Key(4)]
        public Vector3 pos { get; set; }

        [Key(5)]
        public Vector3 moveDir;
    }
}
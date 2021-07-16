using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class btObject
    {
        [Key(0)]
        public btObjectType type;
        [Key(1)]
        public int id;

        [IgnoreMember]
        public Vector3 worldMin;
        [IgnoreMember]
        public Vector3 worldMax;

        [IgnoreMember]
        public q3BodyType bodyType;
        [IgnoreMember]
        public IntPtr body = IntPtr.Zero;
        // public IntPtr box = IntPtr.Zero;

        [IgnoreMember]
        public List<btObject> collidings = new List<btObject>();

        public override string ToString()
        {
            return this.type.ToString() + this.id;
        }
    }
}
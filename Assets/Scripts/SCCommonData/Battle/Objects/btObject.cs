using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public abstract class btObject
    {
        public btObjectType type;
        public int id;
        public Vector3 worldMin;
        public Vector3 worldMax;

        public q3BodyType bodyType;
        public IntPtr body = IntPtr.Zero;
        // public IntPtr box = IntPtr.Zero;

        public List<btObject_Time> collidings { get; } = new List<btObject_Time>();
        
        public virtual void AddToPhysicsScene()
        {

        }

        public override string ToString()
        {
            return this.type.ToString() + this.id;
        }
        // public virtual void Update()
        // {

        // }
    }
}
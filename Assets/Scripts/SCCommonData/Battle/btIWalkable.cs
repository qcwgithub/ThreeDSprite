using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public struct PredictMoveResult
    {
        public bool OutOfRange;
        public float Y;
        //public Vector3 Delta;
    }


    public interface btIWalkable
    {
        PredictMoveResult PredictMove(Vector3 from, Vector3 delta);
        //bool IsInRange(Vector3 pos);
        // Vector3 RandomPos();
        bool CanAccept(Vector3 from, Vector3 delta);
    }
}
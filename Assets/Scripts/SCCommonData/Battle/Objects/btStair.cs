using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum StairDir
    {
        front_back,
        left_high_right_low,
        left_low_right_high,
    }
    public class btStair : btObject, btIWalkable
    {
        public StairDir dir;
    }
}
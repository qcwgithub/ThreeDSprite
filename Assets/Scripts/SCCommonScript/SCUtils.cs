using System.Collections.Generic;
using System;
using Data;

namespace Script
{
    public class SCUtils : GameScriptBase
    {
        /**
         * 返回将value限定于[min,max]区间
         */
        public int clamp(int value, int min, int max)
        {
            if (!(value > min))
            {
                return min;
            }
            if (value < max)
            {
                return value;
            }
            return max;
        }
    }
}
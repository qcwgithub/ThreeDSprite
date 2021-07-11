using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class TypeToMessageCodeCache<T>
    {
        static MessageCode _code;
        static bool inited = false;
        public static MessageCode messageCode
        {
            get
            {
                if (!inited)
                {
                    _code = (MessageCode) Enum.Parse(typeof(MessageCode), typeof(T).Name);
                    inited = true;
                }
                return _code;
            }
        }
    }
}
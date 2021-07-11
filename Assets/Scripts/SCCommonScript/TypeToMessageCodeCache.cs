using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class TypeToMessageCodeCache
    {
        static Dictionary<Type, MessageCode> dict = new Dictionary<Type, MessageCode>();
        public static MessageCode getMessageCode(object obj)
        {
            if (obj == null)
            {
                return MessageCode.MsgNull;
            }

            Type type = obj.GetType();

            MessageCode messageCode;
            if (!dict.TryGetValue(type, out messageCode))
            {
                messageCode = (MessageCode)Enum.Parse(typeof(MessageCode), type.Name);
                dict.Add(type, messageCode);
            }
            return messageCode;
        }
    }
}
using Data;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Script
{
    public class JsonMessagePackerC : JsonMessagePacker
    {
        private static JsonMessagePackerC instance;
        public static JsonMessagePackerC Instance
        {
            get
            {
                if (instance == null) instance = new JsonMessagePackerC();
                return instance;
            }
        }

        private JsonUtils _JSON;
        protected override JsonUtils JSON
        {
            get
            {
                if (_JSON == null) _JSON = new JsonUtils();
                return _JSON;
            }
        }
        Dictionary<string, Type> _name2Type;
        protected override Dictionary<string, Type> name2Type
        {
            get
            {
                // if (_name2Type == null)
                // {
                //     _name2Type = new Dictionary<string, Type>();

                //     // system types
                //     _name2Type.Add(typeof(int).Name, typeof(int));
                //     _name2Type.Add(typeof(string).Name, typeof(string));
                //     //dataEntry.name2Type.Add(typeof(List<int>).Name, typeof(List<int>));
                //     //dataEntry.name2Type.Add(typeof(List<string>).Name, typeof(List<string>));

                //     // data.dll types
                //     var allDataTypes = typeof(Data.MsgType).Assembly.GetTypes();
                //     foreach (var type in allDataTypes)
                //     {
                //         if (typeof(Data.IJsonSerializable).IsAssignableFrom(type))
                //         {
                //             _name2Type.Add(type.Name, type);
                //         }
                //     }
                // }
                return _name2Type;
            }
        }
        protected override void onError(string msg)
        {
            Debug.LogError(msg);
        }
    }
}
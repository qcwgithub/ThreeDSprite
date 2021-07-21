using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Data;

namespace Script
{
    public abstract class DBQuery : DBHandler
    {
        public override MsgType msgType { get { return MsgType.DBQuery; } }
        
        protected MySqlParameter[] MakeParameters(List<object> values)
        {
            var array = new MySqlParameter[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                array[i] = new MySqlParameter("@" + i, values[i]);
            }
            return array;
        }
    }
}
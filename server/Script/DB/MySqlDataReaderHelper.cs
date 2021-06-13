using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script
{
    public class MySqlDataReaderHelper
    {
        MySqlDataReader reader;
        Dictionary<string, object> dict;
        Server server;
        public MySqlDataReaderHelper(Server server, MySqlDataReader reader)
        {
            this.server = server;
            this.reader = reader;
            this.dict = new Dictionary<string, object>();
        }

        public async Task<bool> ReadRow()
        {
            this.dict.Clear();
            if (await this.reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    object value = reader.GetValue(i);
                    this.dict.Add(key, value is DBNull ? null : value);
                }
                return true;
            }
            return false;
        }

        public string GetString(string column)
        {
            return (string)this.dict[column];
        }
        public int GetInt32(string column)
        {
            return (int)this.dict[column];
        }
        public bool GetBoolean(string column)
        {
            return (bool)this.dict[column];
        }
        public int GetDateTimeToSeconds(string column)
        {
            object obj;
            if (!this.dict.TryGetValue(column, out obj) || obj == null)
            {
                return 0;
            }
            DateTime time = (DateTime)obj;
            return this.server.DateTimeToSeconds(time);
        }
    }
}
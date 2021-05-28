using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Data
{
    public class SqlConfig
    {
        public int connectionLimit;
        public string user;
        public string password;
        public string database;
    }

    public class DBData : BaseData
    {
        public SqlConfig sqlConfig;
        public string connectionString;
        public int queryCount = 0;
    }
}
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Data
{
    public sealed class SqlConfig
    {
        public int connectionLimit;
        public string user;
        public string password;
        public string database;
    }

    public sealed class DBData : ServerData
    {
        public SqlConfig sqlConfig;
        public string connectionString;
        public int queryCount = 0;
    }
}
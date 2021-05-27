using MySql.Data.MySqlClient;
using System.Collections.Generic;

public class SqlConfig
{
    public int connectionLimit;
    public string user;
    public string password;
    public string database;
}

public class DBData
{
    public SqlConfig sqlConfig;
    public string connectionString;
    public int queryCount = 0;
}
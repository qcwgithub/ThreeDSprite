public class _SqlConfig
{
    public int connectionLimit;
    public string user;
    public string password;
    public string database;
}

public class DBData
{
    public _SqlConfig sqlConfig;
    public object pool;
    public int queryCount = 0;
}
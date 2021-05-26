using System.Collections.Generic;
public class SqlTablePlayer
{
    public int id;

    //#region autoSqlTablePlayer >>>>>>>>自动生成区域开始
    public string userName; //49/51

    //#endregion autoSqlTablePlayer <<<<<<<<自动生成区域结束

    public static SqlTablePlayer FromRecord(Dictionary<string, List<object>> dict, int index)
    {
        var obj = new SqlTablePlayer();
        obj.id = (int)dict["id"][index];
        obj.userName = (string)dict["userName"][index];
        return obj;
    }
}
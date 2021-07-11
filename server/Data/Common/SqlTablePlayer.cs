using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class SqlTablePlayer
    {
        [Key(0)]
        public int id;

        //#region autoSqlTablePlayer >>>>>>>>自动生成区域开始
        [Key(1)]
        public string userName; //49/51

        //#endregion autoSqlTablePlayer <<<<<<<<自动生成区域结束
    }
}
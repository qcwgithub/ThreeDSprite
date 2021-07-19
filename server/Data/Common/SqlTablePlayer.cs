using System.Collections.Generic;
using MessagePack;
using System.Numerics;

namespace Data
{
    [MessagePackObject]
    public class SqlTablePlayer
    {
        [Key(0)]
        public int id;

        #region SqlTablePlayer Auto

        [Key(1)]
        public int level;
        [Key(2)]
        public string money;
        [Key(3)]
        public int diamond;
        [Key(4)]
        public string portrait;
        [Key(5)]
        public string userName;
        [Key(6)]
        public int characterConfigId;

        #endregion SqlTablePlayer Auto
    }
}
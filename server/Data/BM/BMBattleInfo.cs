using System.Collections.Generic;
using MessagePack;

namespace Data
{
    public partial class BMBattleInfo
    {
        [IgnoreMember]
        public int nextCharacterId = 10000;
    }
}
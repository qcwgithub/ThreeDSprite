using System.Collections.Generic;
using MessagePack;

namespace Data
{
    public partial class BMBattle
    {
        [IgnoreMember]
        public int nextCharacterId = 10000;
    }
}
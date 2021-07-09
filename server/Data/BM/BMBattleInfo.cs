using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data
{
    public partial class BMBattleInfo
    {
        [JsonIgnore]
        public int nextCharacterId = 10000;
    }
}
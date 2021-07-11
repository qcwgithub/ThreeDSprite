using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class LobbyBattleInfo
    {
        [Key(0)]
        public int bmId;
        [Key(1)]
        public int battleId;
        [Key(2)]
        public List<int> playerIds;
        [Key(3)]
        public int mapId;
    }
}
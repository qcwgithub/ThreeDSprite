using System.Collections.Generic;

namespace Data
{
    public class LobbyBMInfo
    {
        public int bmId;
        public int battleCount;
        public bool allowNewBattle;
        public Dictionary<int, LobbyBattleInfo> battles;
    }
}
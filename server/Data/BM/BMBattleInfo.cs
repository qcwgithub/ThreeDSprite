using System.Collections.Generic;

namespace Data
{
    public class BMBattleInfo
    {
        public int battleId;
        public btBattle battle;
        public Dictionary<int, BMPlayerInfo> players = new Dictionary<int, BMPlayerInfo>();
    }
}
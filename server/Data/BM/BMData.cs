using System.Collections.Generic;

namespace Data
{
    public class _BMActive
    {
        public bool first;
        public int count;
        public bool requireBattleList;
        public int timer;
    }
    public sealed class BMData : ServerData
    {
        // id -> BMBattleInfo
        public Dictionary<int, BMBattleInfo> battleInfos = new Dictionary<int, BMBattleInfo>();
        public BMBattleInfo GetBattleInfo(int id)
        {
            BMBattleInfo info;
            if (!this.battleInfos.TryGetValue(id, out info))
            {
                return null;
            }
            return info;
        }

        public _BMActive alive = new _BMActive
        {
            first = true,
            count = 0,
            requireBattleList = false,
        };

        public bool lobbyReady;
        public bool allowNewBattle;
    }
}
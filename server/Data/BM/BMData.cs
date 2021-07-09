using System.Collections.Generic;
using System;

namespace Data
{
    public class _BMActive
    {
        public bool first;
        public int count;
        public bool requireBattleList;
        public int timer;
    }

    public sealed class BMData : ServerData, IBattleConfigs
    {
        // id -> BMBattleInfo
        public Dictionary<int, BMBattleInfo> battleDict = new Dictionary<int, BMBattleInfo>();
        public BMBattleInfo GetBattle(int id)
        {
            BMBattleInfo info;
            if (!this.battleDict.TryGetValue(id, out info))
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
        public bool allowNewBattle = true;

        // 配置文件

        public Dictionary<int, btTilemapData> tilemapDatas { get; } = new Dictionary<int, btTilemapData>();
        public Dictionary<string, btTilesetConfig> tilesetConfigs { get; } = new Dictionary<string, btTilesetConfig>();
    }
}
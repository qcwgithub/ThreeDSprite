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
        // battleId -> BMBattle
        public Dictionary<int, BMBattle> battleDict = new Dictionary<int, BMBattle>();
        public BMBattle GetBattle(int battleId)
        {
            BMBattle battle;
            return this.battleDict.TryGetValue(battleId, out battle) ? battle : null;
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
        public btTilemapData GetTilemapData(int mapId)
        {
            btTilemapData data;
            return this.tilemapDatas.TryGetValue(mapId, out data) ? data : null;
        }

        public Dictionary<string, btTilesetConfig> tilesetConfigs { get; } = new Dictionary<string, btTilesetConfig>();
        public btTilesetConfig GetTilesetConfig(string tileset)
        {
            btTilesetConfig config;
            return this.tilesetConfigs.TryGetValue(tileset, out config) ? config : null;
        }
    }
}
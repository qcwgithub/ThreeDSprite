using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public interface IBattleConfigs
    {
        Dictionary<int, btTilemapData> tilemapDatas { get; }
        Dictionary<string, btTilesetConfig> tilesetConfigs { get; }
    }
}
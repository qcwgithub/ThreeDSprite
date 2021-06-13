using System.Collections.Generic;

namespace Data
{
    public interface IGameConfigs
    {
        int MaxChapter { get; }
        BaseConfigData BaseConfig { get; }
        Dictionary<int, KingdomConfigInfo> KingdomConfig { get; }
        MissionBattleInfos MissionBattles { get; }
        Dictionary<string, ProfileCastle> BattleMissionDict { get; }
        Dictionary<string, ItemConfig> ItemConfig { get; }
        Dictionary<int, List<StrengthenPrice>> StrengthenPrices { get; }
        Dictionary<string, OutputItemConfig> OutputConfig { get; }
        Dictionary<string, DropItemInfo> DropItemDict { get; }
        MissionItemInfos Missions { get; }
        Dictionary<int, KeyValuePair<ChapterInfoForParse, List<MissionItemInfo>>> ChapterMissionDict { get; }
    }
}
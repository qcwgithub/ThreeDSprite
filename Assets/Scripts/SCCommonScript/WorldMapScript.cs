using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class WorldMapScript : GameScriptBase
    {
        public override void Init(IGameConfigs configs, IGameScripts scripts)
        {
            base.Init(configs, scripts);
        }

        /// <summary>
        /// 检查是否battleIDs已经有正确的数据
        /// </summary>
        public void RefreshBattleIDs(IProfileInput input)
        {
            // 长度是否一致
            // 场上一定会有最后两章的数据
            int count = configs.ChapterMissionDict[input.Profile.MapInfo.Chapter].Value.Count;
            count += configs.ChapterMissionDict[input.Profile.MapInfo.Chapter + 1].Value.Count;
            if (input.Profile.MapInfo.BattleIDs.Count == count) { return; } // TODO : 是否还需要检查id的正确性

            // 重新分配
            input.Profile.MapInfo.BattleIDs.Clear();
            input.Profile.MapInfo.BattleIDs.AddRange(GetChapterBattleIDs(configs.ChapterMissionDict[input.Profile.MapInfo.Chapter]));
            input.Profile.MapInfo.BattleIDs.AddRange(GetChapterBattleIDs(configs.ChapterMissionDict[input.Profile.MapInfo.Chapter + 1]));
        }

        public void NewChapterExcute(IProfileInput input)
        {
            // 删除上一章数据,添加下一章数据
            input.Profile.MapInfo.BattleIDs.RemoveRange(0, configs.ChapterMissionDict[input.Profile.MapInfo.Chapter - 1].Key.battleCount);
            input.Profile.MapInfo.BattleIDs.AddRange(GetChapterBattleIDs(configs.ChapterMissionDict[input.Profile.MapInfo.Chapter + 1]));
        }

        private string[] GetChapterBattleIDs(KeyValuePair<ChapterInfoForParse, List<MissionItemInfo>> pair)
        {
            var chapter = pair.Key;
            var infos = pair.Value;
            int count = chapter.battleCount + chapter.eventCount;
            List<string> battles = new List<string>();

            int battleIndex = 0;
            for (int i = 1; i < chapter.id; ++i)
            {
                battleIndex += configs.ChapterMissionDict[i].Key.battleCount;
            }
            for (int i = 0; i < chapter.battleCount; ++i)
            {
                battles.Add(configs.MissionBattles.infos[battleIndex++].battleID);
            }

            Random rand = new Random();
            int eventCount = chapter.eventCount;
            int index = chapter.battleCount - 1;    // index==0不能插入
            while (index > 0 && eventCount > 0)
            {
                if (rand.Next(0, index * 2) < eventCount)
                {
                    battles.Insert(index, "");
                    --eventCount;
                }
                if (rand.Next(0, index * 2 - 1) < eventCount)
                {
                    battles.Insert(index, "");
                    --eventCount;
                }
                --index;
            }
            return battles.ToArray();
        }
    }
}
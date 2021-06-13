using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class ConfigScript
    {
        public static void ProcesKingdomConfig(string kingdomText, out Dictionary<int, KingdomConfigInfo> dict)
        {
            dict = new Dictionary<int, KingdomConfigInfo>();
            KingdomConfigInfos info = (KingdomConfigInfos)CSVParseUtils.ParseCSVToObj("KingdomConfigInfos", CSVParseUtils.ParseCsvData(kingdomText));
            foreach (var item in info.infos)
            {
                dict.Add(item.level, item);
            }
        }
        static ItemConfig CreateItemConfig(ItemConfigDataParse item)
        {
            var _this = new ItemConfig();
            _this.id = item.id;
            _this.hp = CSVParseUtils.BigIntegerParse(item.hp);
            _this.defense = CSVParseUtils.BigIntegerParse(item.defense);
            _this.damage = CSVParseUtils.BigIntegerParse(item.damage);

            _this.strengthenHP = CSVParseUtils.BigIntegerParse(item.strengthenHP);
            _this.strengthenDEF = CSVParseUtils.BigIntegerParse(item.strengthenDEF);
            _this.strengthenATK = CSVParseUtils.BigIntegerParse(item.strengthenATK);

            _this.maxLevel = item.maxLevel;
            _this.weight = item.weight;
            _this.rank = 1;
            _this.cdTimeS = item.cdTimeS;
            _this.output = item.output;
            _this.priorities = item.weaponPrior;
            _this.count = item.count;
            _this.speed = item.speed;
            _this.itemIcon = item.itemIcon;
            _this.itemPfb = item.itemPfb;
            _this.mergeTo = item.mergeTo;
            _this.angle = item.angle;
            _this.type = (BackpackItemType)Enum.Parse(typeof(BackpackItemType), item.type);
            return _this;
        }

        static ItemConfig CreateItemConfig(CastleRoomConfigDataParse item)
        {
            var _this = new ItemConfig();
            if (string.IsNullOrEmpty(item.hp)) { item.hp = "0"; }
            if (string.IsNullOrEmpty(item.strengthenHP)) { item.strengthenHP = "0"; }

            _this.id = item.id;
            _this.hp = CSVParseUtils.BigIntegerParse(item.hp);
            _this.strengthenHP = CSVParseUtils.BigIntegerParse(item.strengthenHP);
            _this.maxLevel = item.maxLevel;
            _this.weight = item.weight;
            _this.rank = 1;
            _this.itemIcon = item.itemIcon;
            _this.itemPfb = item.itemPfb;
            _this.gatePfb = item.gatePfb;
            _this.topPfb = item.topPfb;
            _this.mergeTo = item.mergeTo;
            _this.type = BackpackItemType.Room;
            return _this;
        }
        public static void ProcesItemConfig(string roomText, string itemText, out Dictionary<string, ItemConfig> cubeDict)
        {
            cubeDict = new Dictionary<string, ItemConfig>();
            ItemConfig preItem = null;

            CastleRoomConfigDataParses rooms = (CastleRoomConfigDataParses)CSVParseUtils.ParseCSVToObj("CastleRoomConfigDataParses", CSVParseUtils.ParseCsvData(roomText));
            ItemConfigDataParses items = (ItemConfigDataParses)CSVParseUtils.ParseCSVToObj("ItemConfigDataParses", CSVParseUtils.ParseCsvData(itemText));

            foreach (var item in items.items)
            {
                var newItem = CreateItemConfig(item);
                if (preItem != null && !string.IsNullOrEmpty(preItem.mergeTo) && preItem.mergeTo.Equals(newItem.id))
                {
                    newItem.rank += preItem.rank;
                }

                cubeDict.Add(item.id, newItem);
                preItem = newItem;
            }
            preItem = null;
            foreach (var item in rooms.items)
            {
                var newItem = CreateItemConfig(item);
                if (preItem != null && !string.IsNullOrEmpty(preItem.mergeTo) && preItem.mergeTo.Equals(newItem.id))
                {
                    newItem.rank += preItem.rank;
                }

                cubeDict.Add(item.id, newItem);
                preItem = newItem;
            }
        }

        public static void ProcessOutputConfig(string outputText, out Dictionary<string, OutputItemConfig> outputDict)
        {
            outputDict = new Dictionary<string, OutputItemConfig>();
            OutputItems outputs = (OutputItems)CSVParseUtils.ParseCSVToObj("OutputItems", CSVParseUtils.ParseCsvData(outputText));

            foreach (var item in outputs.outputItems)
            {
                outputDict.Add(item.id, new OutputItemConfig(item));
            }
        }

        static MissionItemInfo CreateMissionItemInfo(MissionInfoForParse infoSO)
        {
            var _this = new MissionItemInfo();
            if (string.IsNullOrEmpty(infoSO.winMoney)) { infoSO.winMoney = "0"; }
            if (string.IsNullOrEmpty(infoSO.winReinforced)) { infoSO.winReinforced = "0"; }

            _this.missionID = infoSO.missionID;

            _this.materialInfo = new MaterialInfo();  // 通关奖励
            _this.materialInfo.diamond = infoSO.winDiamond;
            _this.materialInfo.money = CSVParseUtils.BigIntegerParse(infoSO.winMoney);
            _this.materialInfo.strengthenMate = CSVParseUtils.BigIntegerParse(infoSO.winReinforced);
            _this.materialInfo.advancedMate = infoSO.winAdvanced;

            _this.mopUpMaterialInfo = new MaterialInfo(); // 扫荡奖励，目前扫荡奖励就是不给钻石
            _this.materialInfo.money = CSVParseUtils.BigIntegerParse(infoSO.winMoney);
            _this.materialInfo.strengthenMate = CSVParseUtils.BigIntegerParse(infoSO.winReinforced);
            _this.materialInfo.advancedMate = infoSO.winAdvanced;

            _this.mopUpDiamondRate = infoSO.mopUpDiamondRate;

            _this.materialType = (MaterialType)Enum.Parse(typeof(MaterialType), infoSO.materialType);
            return _this;
        }

        public static void ProcessMissionInfo(string infoText, string chapterText, out MissionItemInfos infos, out Dictionary<int, KeyValuePair<ChapterInfoForParse, List<MissionItemInfo>>> chapterMissionDict, ref int maxChapter)
        {
            MissionInfoForParses data = (MissionInfoForParses)CSVParseUtils.ParseCSVToObj("MissionInfoForParses", CSVParseUtils.ParseCsvData(infoText));
            ChapterInfoForParses chapters = (ChapterInfoForParses)CSVParseUtils.ParseCSVToObj("ChapterInfoForParses", CSVParseUtils.ParseCsvData(chapterText));

            infos = new MissionItemInfos();
            chapterMissionDict = new Dictionary<int, KeyValuePair<ChapterInfoForParse, List<MissionItemInfo>>>();

            for (int i = 0; i < data.missionInfos.Count; ++i)
            {
                MissionItemInfo mission = CreateMissionItemInfo(data.missionInfos[i]);
                mission.missionIndex = i;
                infos.AddListItem(mission);
            }

            int index = 0;
            foreach (var chapter in chapters.infos)
            {
                int count = chapter.battleCount + chapter.eventCount;
                chapterMissionDict.Add(chapter.id, new KeyValuePair<ChapterInfoForParse, List<MissionItemInfo>>(chapter, new List<MissionItemInfo>()));
                maxChapter = chapter.id;

                var missionInfos = chapterMissionDict[chapter.id].Value;

                while (count > 0)
                {
                    var mission = infos.missions[index];
                    mission.chapter = chapter.id;
                    missionInfos.Add(mission);
                    ++index;
                    --count;
                }
            }
        }

        public static void ProcessMissionBattle(string battleItemText, Dictionary<string, ItemConfig> itemConfig, out Dictionary<string, ProfileCastle> battleMissionDict, out MissionBattleInfos missionBattles)
        {
            missionBattles = CSVParseUtils.ParseCastleArray(CSVParseUtils.ParseCsvData(battleItemText), itemConfig);
            battleMissionDict = new Dictionary<string, ProfileCastle>();
            foreach (var item in missionBattles.infos)
            {
                battleMissionDict.Add(item.battleID, new ProfileCastle(item.castle));
            }
        }

        static StrengthenPrice CreateStrengthenPrice(StrengthenPriceItem info)
        {
            var _this = new StrengthenPrice();
            _this.rank = info.rank;
            _this.level = info.level;
            _this.money = CSVParseUtils.BigIntegerParse(info.money);
            _this.strengthenMate = CSVParseUtils.BigIntegerParse(info.strengthenMate);
            _this.advancedMate = info.advancedMate;
            return _this;
        }

        public static void ProcessStrengthenPrice(string strengthenPriceText, out Dictionary<int, List<StrengthenPrice>> dict)
        {
            StrengthenPriceItems strengthenPrices = (StrengthenPriceItems)CSVParseUtils.ParseCSVToObj("StrengthenPriceItems", CSVParseUtils.ParseCsvData(strengthenPriceText));
            dict = new Dictionary<int, List<StrengthenPrice>>();

            foreach (var item in strengthenPrices.infos)
            {
                if (!dict.ContainsKey(item.rank)) { dict.Add(item.rank, new List<StrengthenPrice>()); }

                dict[item.rank].Add(CreateStrengthenPrice(item));
            }
        }

        public static void ProcessDropItem(string dropItemText, out Dictionary<string, DropItemInfo> dict)
        {
            DropItemInfos infos = CSVParseUtils.ParseDropItemArray(CSVParseUtils.ParseCsvData(dropItemText));
            dict = new Dictionary<string, DropItemInfo>();
            foreach (var item in infos.infos)
            {
                dict.Add(item.dropID, item);
            }
        }
    }
}
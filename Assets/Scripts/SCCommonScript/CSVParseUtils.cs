using System;
using System.Numerics;
using System.Collections.Generic;
using System.Reflection;
using Data;

public class CSVParseUtils
{
    private const char CSVSplit = ',';
    public static System.Object ParseCSVToObj(string dataName, string[][] array)
    {
        string className = dataName.Substring(0, dataName.Length - 1);
        Type dataType = Type.GetType(dataName);
        object dataObj = Activator.CreateInstance(dataType);

        for (int m = 2; m < array.Length - 1; m++)
        {
            if (string.IsNullOrEmpty(array[m][0]) || array[m][0].StartsWith("#")) { continue; } // 忽略#号开头的行
            Type classType = Type.GetType(className);
            object classObj = Activator.CreateInstance(classType);
            FieldInfo[] fis = classType.GetFields();
            for (int i = 0; i < fis.Length; i++)
            {
                string[] strs = fis[i].ToString().Split(' ');
                switch (strs[0])
                {
                    case "Data.WeaponPriorities":
                        fis[i].SetValue(classObj, WeaponPrioritiesParse(array[m][i]));
                        break;
                    case "System.Int32[]":
                        fis[i].SetValue(classObj, IntArrayParse(array[m][i]));
                        break;
                    case "System.String[]":
                        fis[i].SetValue(classObj, StringArrayParse(array[m][i]));
                        break;
                    case "System.Int32":
                        fis[i].SetValue(classObj, IntParse(array[m][i]));
                        break;
                    case "System.Int64":
                        fis[i].SetValue(classObj, LongParse(array[m][i]));
                        break;
                    case "System.String":
                        fis[i].SetValue(classObj, array[m][i]);
                        break;
                    case "System.Numerics.BigInteger":
                        fis[i].SetValue(classObj, BigInteger.Parse(array[m][i]));
                        break;
                    case "Data.RewardItemInfo":
                        fis[i].SetValue(classObj, RewardItemInfoParse(array[m][i]));
                        break;
                    case "Data.RewardItemInfo[]":
                        fis[i].SetValue(classObj, RewardItemInfoArrayParse(array[m][i]));
                        break;
                    case "Data.ItemWeightInfo":
                        fis[i].SetValue(classObj, ItemWeightInfoParse(array[m][i]));
                        break;
                    case "Data.ItemWeightInfo[]":
                        fis[i].SetValue(classObj, ItemWeightInfoArrayParse(array[m][i]));
                        break;
                    default:
                        break;
                }
            }

            MethodInfo methodInfo = dataType.GetMethod("AddListItem");
            object[] parameters = new object[] { classObj };
            methodInfo.Invoke(dataObj, parameters);
        }
        return dataObj;
    }

    public static string[][] ParseCsvData(string text)
    {
        text = text.Replace("\"", "");    // 删引号
        text = text.Replace(" ", "");     // 删空格
        text = text.Replace("\r", "");    // 去换行
        string[] lineArray = text.Split('\n');
        string[][] array = new string[lineArray.Length][];

        for (int i = 0; i < lineArray.Length; i++)
        {
            array[i] = lineArray[i].Split(CSVSplit);
        }
        return array;
    }

    public static WeaponPriorities WeaponPrioritiesParse(string str)
    {
        WeaponPriorities prior = new WeaponPriorities();
        var array = IntArrayParse(str);
        if (array == null || array.Length != 3)
        {
            return prior;
        }
        prior.troops = array[0];
        prior.weapon = array[1];
        prior.king = array[2];
        return prior;
    }

    public static int IntParse(string str, int defaultValue = 0)
    {
        return string.IsNullOrEmpty(str) ? defaultValue : int.Parse(str);
    }

    public static long LongParse(string str, long defaultValue = 0)
    {
        return string.IsNullOrEmpty(str) ? defaultValue : long.Parse(str);
    }

    public static int[] IntArrayParse(string str)
    {
        if (string.IsNullOrEmpty(str)) { return null; }

        var nums = str.Split(';');
        var result = new int[nums.Length];
        for (int i = 0; i < nums.Length; ++i)
        {
            result[i] = int.Parse(nums[i]);
        }
        return result;
    }

    public static string[] StringArrayParse(string str)
    {
        if (string.IsNullOrEmpty(str)) { return null; }

        return str.Split(';');
    }

    public static BigInteger BigIntegerParse(string str)
    {
        if (string.IsNullOrEmpty(str)) { return BigInteger.Zero; }

        return BigInteger.Parse(str);
    }

    public static ItemWeightInfo[] ItemWeightInfoArrayParse(string str)
    {
        var infoStrs = StringArrayParse(str);
        ItemWeightInfo[] infos = new ItemWeightInfo[infoStrs.Length];
        for (int i = 0; i < infoStrs.Length; ++i)
        {
            infos[i] = ItemWeightInfoParse(infoStrs[i]);
        }
        return infos;
    }

    public static ItemWeightInfo ItemWeightInfoParse(string str)
    {
        ItemWeightInfo info = new ItemWeightInfo();
        var split = str.Split('|');
        info.itemWeight = IntParse(split[0]);
        info.weight = IntParse(split[1]);
        return info;
    }

    public static RewardItemInfo RewardItemInfoParse(string str)   // str的格式为：物品ID*个数|权重；其中ID为"NULL"是空物品，抽到没奖励；权重不填则表示用不到，比如首通奖励
    {
        RewardItemInfo info = new RewardItemInfo();
        var split1 = str.Split('*');
        if ("NULL".Equals(split1[0])) { return info; }

        info.itemID = split1[0];
        if (split1.Length != 2) // 没填数量
        {
            info.count = 1;
            var splits = split1[0].Split('|');
            if (splits.Length == 2) { info.weight = IntParse(splits[1]); }
            return info;
        }

        var split2 = split1[1].Split('|');
        info.count = IntParse(split2[0]);
        if (split2.Length == 2) { info.weight = IntParse(split2[1]); }
        return info;
    }

    public static RewardItemInfo[] RewardItemInfoArrayParse(string str)
    {
        var infoStrs = StringArrayParse(str);
        RewardItemInfo[] infos = new RewardItemInfo[infoStrs.Length];
        for (int i = 0; i < infoStrs.Length; ++i)
        {
            infos[i] = RewardItemInfoParse(infoStrs[i]);
        }
        return infos;
    }

    public static MissionBattleInfos ParseCastleArray(string[][] csvArray, Dictionary<string, ItemConfig> itemConfig)
    {
        MissionBattleInfos data = new MissionBattleInfos();
        for (int start = 0; start < csvArray.Length; start += 8)
        {
            List<BuildingItem> list = new List<BuildingItem>();
            for (int x = 4; x >= 0; --x)
            {
                for (int y = start + 6; y > start; --y)
                {
                    if ("+".Equals(csvArray[y][x])) { continue; }

                    var pos = new IntVec2(x - 2, start + 6 - y);
                    var items = csvArray[y][x].Split('+');

                    BuildingItem item = BuildingItemData(items[0], pos, false);
                    list.Add(item);
                    if (itemConfig.ContainsKey(item.id) && itemConfig[item.id].type == BackpackItemType.Room && items.Length >= 2)
                    {
                        list.Add(BuildingItemData(items[1], pos, true));
                    }
                }
            }
            MissionBattleInfo info = new MissionBattleInfo();
            info.castle = new ProfileCastle();
            info.castle.Items = list;
            info.battleID = csvArray[start][0];
            data.AddListItem(info);
        }
        return data;
    }

    public static DropItemInfos ParseDropItemArray(string[][] csvArray)
    {
        DropItemInfos data = new DropItemInfos();
        var length = csvArray[0].Length;
        for (int i = 0; i < length; i += 3)
        {
            if (string.IsNullOrEmpty(csvArray[0][i])) { continue; }

            var dropItem = new DropItemInfo();
            dropItem.dropID = csvArray[0][i];
            dropItem.items = new List<RewardItemInfo>();

            for (int j = 2; j < csvArray.Length; ++j)
            {
                if (csvArray[j].Length != length) { break; }
                var info = new RewardItemInfo();
                if (!"NULL".Equals(csvArray[j][i]))
                {
                    info.itemID = csvArray[j][i];
                    info.count = IntParse(csvArray[j][i + 1]);
                    info.weight = IntParse(csvArray[j][i + 2]);
                }
                dropItem.items.Add(info);
            }
            data.AddListItem(dropItem);
        }
        return data;
    }

    private static BuildingItem BuildingItemData(string str, IntVec2 pos, bool isInterior)
    {
        var strs = str.Split('*');
        BuildingItem item = new BuildingItem();
        item.id = strs[0];
        item.position = pos;
        item.interior = isInterior;
        item.connectToRight = true;
        if (strs.Length == 2) { item.level = int.Parse(strs[1]); }
        return item;
    }
}

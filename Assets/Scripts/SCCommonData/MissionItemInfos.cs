using System;
using System.Numerics;
using System.Collections.Generic;
using Data;

public class MissionItemInfos
{
    public List<MissionItemInfo> missions = new List<MissionItemInfo>();
    public void AddListItem(MissionItemInfo data)
    {
        missions.Add(data);
    }
}

[Serializable]
public class MissionItemInfo
{
    public string missionID;
    public int chapter;
    public int missionIndex;
    public MaterialInfo materialInfo;
    public MaterialInfo mopUpMaterialInfo;
    public int mopUpDiamondRate;
    public MaterialType materialType;


    // public MissionItemInfo(MissionInfoForParse infoSO)
    // {
    //     if (string.IsNullOrEmpty(infoSO.winMoney)) { infoSO.winMoney = "0"; }
    //     if (string.IsNullOrEmpty(infoSO.winReinforced)) { infoSO.winReinforced = "0"; }

    //     missionID = infoSO.missionID;

    //     materialInfo = new MaterialInfo();  // 通关奖励
    //     materialInfo.diamond = infoSO.winDiamond;
    //     materialInfo.money = CSVParseUtils.BigIntegerParse(infoSO.winMoney);
    //     materialInfo.strengthenMate = CSVParseUtils.BigIntegerParse(infoSO.winReinforced);
    //     materialInfo.advancedMate = infoSO.winAdvanced;

    //     mopUpMaterialInfo = new MaterialInfo(); // 扫荡奖励，目前扫荡奖励就是不给钻石
    //     materialInfo.money = CSVParseUtils.BigIntegerParse(infoSO.winMoney);
    //     materialInfo.strengthenMate = CSVParseUtils.BigIntegerParse(infoSO.winReinforced);
    //     materialInfo.advancedMate = infoSO.winAdvanced;

    //     mopUpDiamondRate = infoSO.mopUpDiamondRate;

    //     materialType = (MaterialType)Enum.Parse(typeof(MaterialType), infoSO.materialType);
    // }
}

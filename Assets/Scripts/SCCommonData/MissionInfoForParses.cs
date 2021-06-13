using System;
using System.Collections.Generic;

public class MissionInfoForParses
{
    public List<MissionInfoForParse> missionInfos = new List<MissionInfoForParse>();
    public void AddListItem(MissionInfoForParse data)
    {
        missionInfos.Add(data);
    }
}

[Serializable]
public class MissionInfoForParse
{
    public string missionID;
    public string materialType;
    public int winDiamond;
    public string winMoney;
    public string winReinforced;
    public int winAdvanced;
    public int mopUpDiamondRate;
}

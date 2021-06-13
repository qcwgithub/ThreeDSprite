using System;
using System.Collections.Generic;
using Data;

public class MissionBattleInfos
{
    public List<MissionBattleInfo> infos = new List<MissionBattleInfo>();
    public void AddListItem(MissionBattleInfo data)
    {
        infos.Add(data);
    }
}

[Serializable]
public class MissionBattleInfo
{
    public string battleID;
    public ProfileCastle castle;
}

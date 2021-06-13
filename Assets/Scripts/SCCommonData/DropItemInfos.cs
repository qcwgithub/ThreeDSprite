using System;
using System.Collections.Generic;
using Data;

public class DropItemInfos
{
    public List<DropItemInfo> infos = new List<DropItemInfo>();
    public void AddListItem(DropItemInfo data)
    {
        infos.Add(data);
    }
}

[Serializable]
public class DropItemInfo
{
    public string dropID;
    public List<RewardItemInfo> items;
}

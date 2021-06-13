using System;
using System.Collections.Generic;

public class StrengthenPriceItems
{
    public List<StrengthenPriceItem> infos = new List<StrengthenPriceItem>();
    public void AddListItem(StrengthenPriceItem data)
    {
        infos.Add(data);
    }
}


[Serializable]
public class StrengthenPriceItem
{
    public int rank;
    public int level;
    public string money;
    public string strengthenMate;
    public int advancedMate;
}

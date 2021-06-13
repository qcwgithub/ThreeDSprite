using System;
using System.Collections.Generic;

public partial class KingdomConfigInfos
{
    public List<KingdomConfigInfo> infos = new List<KingdomConfigInfo>();
    public void AddListItem(KingdomConfigInfo data)
    {
        infos.Add(data);
    }
}

[Serializable]
public class KingdomConfigInfo
{
    public int level;
    public int maxEXP;
    public int offlineEXPH;
    public int maxWeight;
}
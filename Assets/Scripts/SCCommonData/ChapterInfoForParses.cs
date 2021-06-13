using System;
using System.Numerics;
using System.Collections.Generic;

public class ChapterInfoForParses
{
    public List<ChapterInfoForParse> infos = new List<ChapterInfoForParse>();
    public void AddListItem(ChapterInfoForParse data)
    {
        infos.Add(data);
    }
}

[Serializable]
public class ChapterInfoForParse
{
    public int id;
    public BigInteger fogExpand;
    public int eventCount;
    public int battleCount;
}

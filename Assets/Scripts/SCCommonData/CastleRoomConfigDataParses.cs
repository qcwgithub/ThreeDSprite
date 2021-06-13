using System;
using System.Collections.Generic;

public partial class CastleRoomConfigDataParses
{
    public List<CastleRoomConfigDataParse> items = new List<CastleRoomConfigDataParse>();
    public void AddListItem(CastleRoomConfigDataParse data)
    {
        items.Add(data);
    }
}

[Serializable]
public class CastleRoomConfigDataParse
{
    public string id;
    public string hp;
    public string strengthenHP;
    public int maxLevel;
    public int weight;
    public string mergeTo;
    public string itemIcon;
    public string itemPfb;
    public string gatePfb;
    public string topPfb;
}
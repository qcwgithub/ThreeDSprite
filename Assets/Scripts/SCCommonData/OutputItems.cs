using System;
using System.Collections.Generic;

public partial class OutputItems
{
    public List<OutputItem> outputItems = new List<OutputItem>();
    public void AddListItem(OutputItem data)
    {
        outputItems.Add(data);
    }
}

[Serializable]
public class OutputItem
{
    public string id;
    public int duration;
    public int cdTimeS;
    public int speed;
    public string itemIcon;
    public string itemPfb;
}
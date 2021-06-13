using System;
using System.Collections.Generic;
using Data;

public partial class ItemConfigDataParses
{
    public List<ItemConfigDataParse> items = new List<ItemConfigDataParse>();
    public void AddListItem(ItemConfigDataParse data)
    {
        items.Add(data);
    }
}

[Serializable]
public class ItemConfigDataParse
{
    public string id;
    public string type;
    public string hp;
    public string defense;
    public string damage;
    public int damageFloat;
    public string strengthenHP;
    public string strengthenDEF;
    public string strengthenATK;
    public int maxLevel;
    public int weight;
    public int cdTimeS;
    public int count;
    public string output;
    public int speed;
    public int angle;
    public WeaponPriorities weaponPrior;
    public string mergeTo;
    public string itemIcon;
    public string itemPfb;
}
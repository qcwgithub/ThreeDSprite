using System;
using System.Numerics;

namespace Data
{
    [Serializable]
    public class ItemConfig
    {
        public string id;
        public BigInteger hp;
        public BigInteger strengthenHP;
        public int maxLevel;
        public int weight;
        public int rank;
        public string mergeTo;
        public string itemIcon;
        public string itemPfb;
        public BackpackItemType type;
        public int count;
        public float cdTimeS = 0;
        public string output;
        public int speed;
        public int angle;
        public BigInteger defense;
        public BigInteger damage;
        public int damageFloat;
        public BigInteger strengthenDEF;
        public BigInteger strengthenATK;
        public WeaponPriorities priorities;
        public string gatePfb;
        public string topPfb;

        // public ItemConfig(ItemConfigDataParse item)
        // {
        //     id = item.id;
        //     hp = CSVParseUtils.BigIntegerParse(item.hp);
        //     defense = CSVParseUtils.BigIntegerParse(item.defense);
        //     damage = CSVParseUtils.BigIntegerParse(item.damage);

        //     strengthenHP = CSVParseUtils.BigIntegerParse(item.strengthenHP);
        //     strengthenDEF = CSVParseUtils.BigIntegerParse(item.strengthenDEF);
        //     strengthenATK = CSVParseUtils.BigIntegerParse(item.strengthenATK);

        //     maxLevel = item.maxLevel;
        //     weight = item.weight;
        //     rank = 1;
        //     cdTimeS = item.cdTimeS;
        //     output = item.output;
        //     priorities = item.weaponPrior;
        //     count = item.count;
        //     speed = item.speed;
        //     itemIcon = item.itemIcon;
        //     itemPfb = item.itemPfb;
        //     mergeTo = item.mergeTo;
        //     angle = item.angle;
        //     type = (BackpackItemType)Enum.Parse(typeof(BackpackItemType), item.type);
        // }

        // public ItemConfig(CastleRoomConfigDataParse item)
        // {
        //     if (string.IsNullOrEmpty(item.hp)) { item.hp = "0"; }
        //     if (string.IsNullOrEmpty(item.strengthenHP)) { item.strengthenHP = "0"; }

        //     id = item.id;
        //     hp = CSVParseUtils.BigIntegerParse(item.hp);
        //     strengthenHP = CSVParseUtils.BigIntegerParse(item.strengthenHP);
        //     maxLevel = item.maxLevel;
        //     weight = item.weight;
        //     rank = 1;
        //     itemIcon = item.itemIcon;
        //     itemPfb = item.itemPfb;
        //     gatePfb = item.gatePfb;
        //     topPfb = item.topPfb;
        //     mergeTo = item.mergeTo;
        //     type = BackpackItemType.Room;
        // }
    }

    [Serializable]

    public class OutputItemConfig
    {
        public string id;
        public int duration;
        public int speed;
        public float cdTimeS = 0;
        public string itemIcon;
        public string itemPfb;

        public OutputItemConfig(OutputItem item)
        {
            id = item.id;
            cdTimeS = item.cdTimeS;
            speed = item.speed;
            duration = item.duration;
            itemIcon = item.itemIcon;
            itemPfb = item.itemPfb;
        }
    }

    [Serializable]
    public class WeaponPriorities
    {
        public int troops;
        public int weapon;
        public int king;
    }
}
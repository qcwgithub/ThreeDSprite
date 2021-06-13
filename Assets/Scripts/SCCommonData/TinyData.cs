using System;
using System.Collections.Generic;
using System.Numerics;

namespace Data
{
    public class CoordPos
    {
        public IntVec2 coord;
        public Vector2 pos;
        public CoordPos() { }
        public CoordPos(IntVec2 coord, Vector2 pos)
        {
            this.coord = coord;
            this.pos = pos;
        }
    }


    [Serializable]
    public class RangeValue
    {
        public int min;
        public int max;
        public RangeValue() { }
        public override string ToString()
        {
            return min.ToString() + "~" + max.ToString();
        }
    }

    [Serializable]
    public class ItemData : IComparable<ItemData>
    {
        public string id;
        public int count;
        public ItemConfig cfg;

        public ItemData() { }
        public ItemData(ItemConfig cfg, int count)
        {
            this.id = cfg.id;
            this.count = count;
            this.cfg = cfg;
        }

        int IComparable<ItemData>.CompareTo(ItemData other)
        {
            if (cfg.rank == other.cfg.rank)
            {
                if (cfg.type == other.cfg.type)
                {
                    return id.CompareTo(other.id);
                }
                return (int)cfg.type > (int)other.cfg.type ? 1 : -1;
            }
            return cfg.rank < other.cfg.rank ? 1 : -1;
        }
    }

    [Serializable]
    public class ItemInfo
    {
        public string itemID;
        public List<ItemInfoData> infos;
        public ItemInfo() { }
    }

    public class ItemInfoData : IComparable<ItemInfoData>
    {
        public int level;

        public ItemInfoData() { }
        public ItemInfoData(int level)
        {
            this.level = level;
        }

        public static bool operator !=(ItemInfoData a, ItemInfoData b)
        {
            return !(a == b);
        }
        public static bool operator ==(ItemInfoData a, ItemInfoData b)
        {
            if (object.Equals(a, null) || object.Equals(b, null)) { return object.Equals(a, b); }
            return a.level == b.level;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemInfoData)) { return false; }

            return (ItemInfoData)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        int IComparable<ItemInfoData>.CompareTo(ItemInfoData other)
        {
            if (level == other.level) { return 0; }
            return level < other.level ? 1 : -1;
        }
    }

    public class BackpackItemData
    {
        public ItemInfoData info;
        public ItemData data;
        public BackpackItemData() { }
        public BackpackItemData(ItemData data, ItemInfoData level)
        {
            this.info = level;
            this.data = data;
        }
    }

    public class ItemInfoAndCfg
    {
        public ItemConfig cfg;
        public ItemInfoData info;
        public ItemInfoAndCfg(ItemConfig cfg, ItemInfoData info)
        {
            this.cfg = cfg;
            this.info = info;
        }
    }

    public class ItemAttribute
    {
        public System.Numerics.BigInteger HP;
        public System.Numerics.BigInteger DEF;
        public System.Numerics.BigInteger ATK;

        public ItemAttribute(System.Numerics.BigInteger hp, System.Numerics.BigInteger def, System.Numerics.BigInteger atk)
        {
            this.HP = hp;
            this.DEF = def;
            this.ATK = atk;
        }
    }

    [Serializable]
    public class RewardItemInfo
    {
        public string itemID;
        public int count;
        public int weight;
    }

    [Serializable]
    public class ItemWeightInfo
    {
        public int itemWeight;
        public int weight;
    }

    public class StrengthenPrice
    {
        public int rank;
        public int level;
        public System.Numerics.BigInteger money;
        public System.Numerics.BigInteger strengthenMate;
        public int advancedMate;

        public StrengthenPrice()
        {
            rank = 0;
            level = 0;
            money = 0;
            strengthenMate = 0;
            advancedMate = 0;
        }

        // public StrengthenPrice(StrengthenPriceItem info)
        // {
        //     rank = info.rank;
        //     level = info.level;
        //     money = CSVParseUtils.BigIntegerParse(info.money);
        //     strengthenMate = CSVParseUtils.BigIntegerParse(info.strengthenMate);
        //     advancedMate = info.advancedMate;
        // }
    }

    public class BattlefieldItemPublishData
    {
        public BattlefieldItemPublishData(CastleDirection direction, IntVec2 pos, string itemID)
        {
            this.direction = direction;
            this.pos = pos;
            this.itemID = itemID;
        }
        public CastleDirection direction;
        public IntVec2 pos;
        public string itemID;
    }

    [Serializable]
    public class IntVec2
    {
        public int x;
        public int y;
        private const int outRangeValue = -9999;
        public IntVec2(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsZero() { return x == 0 && y == 0; }
        public bool IsNegativeOne() { return x == -1 && y == -1; }
        public bool IsOutOfRange() { return x == outRangeValue && y == outRangeValue; }
        public IntVec2 Up() { return new IntVec2(x, y + 1); }
        public IntVec2 Down() { return new IntVec2(x, y - 1); }
        public IntVec2 Left() { return new IntVec2(x - 1, y); }
        public IntVec2 Right() { return new IntVec2(x + 1, y); }
        public string Name() { return x.ToString() + y.ToString(); }

        public static IntVec2 NegativeOne { get { return new IntVec2(-1, -1); } }
        public static IntVec2 OutOfRange { get { return new IntVec2(outRangeValue, outRangeValue); } }
        public static IntVec2 zero { get { return new IntVec2(0, 0); } }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            IntVec2 pos = obj as IntVec2;
            return pos != null && pos.x == x && pos.y == y;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
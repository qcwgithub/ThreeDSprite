using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class ProfileCastle
    {
        public List<BuildingItem> Items;

        public ProfileCastle() { }
        public ProfileCastle(ProfileCastle obj)
        {
            Items = new List<BuildingItem>();

            Items = obj.Items;
            if (Items.Count > 0)
            {
                Items.Sort();
            }
        }

        public static ProfileCastle Ensure(ProfileCastle obj)
        {
            if (obj == null)
            {
                obj = new ProfileCastle();
            }

            if (obj.Items == null)
            {
                obj.Items = new List<BuildingItem>();
            }
            else
            {
                obj.Items.Sort();
            }
            return obj;
        }

        public void PositionLevelUp(IntVec2 pos, bool isInterior)
        {
            foreach (var item in Items)
            {
                if (pos.Equals(item.position) && item.interior == isInterior)
                {
                    item.level++;
                    return;
                }
            }
        }

        public void AddItem(BuildingItem data)
        {
            data.level = data.level > 0 ? data.level : 1;
            Items.Add(data);
            Items.Sort();
        }

        public void RemoveItemAt(int index)
        {
            var item = Items[index];
            Items.RemoveAt(index);
        }
    }

    [Serializable]
    public class BuildingItem : IComparable<BuildingItem>
    {
        public IntVec2 position;  // 位置以king为原点
        public string id;
        public int level;
        public bool connectToRight;
        public bool interior;   // 仅仅是排序用

        public BuildingItem() { }
        int IComparable<BuildingItem>.CompareTo(BuildingItem other)
        {
            if (position.x == other.position.x)
            {
                if (position.y == other.position.y)
                {
                    return interior ? 1 : -1;
                }
                return position.y > other.position.y ? 1 : -1;
            }

            return position.x > other.position.x ? -1 : 1;
        }

    }
}
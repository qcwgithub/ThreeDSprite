using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class ProfileBackpack
    {
        public List<ItemData> Items;
        public List<ItemInfo> Infos;

        public ProfileBackpack() { }
        public ProfileBackpack(ProfileBackpack obj)
        {
            Items = obj.Items;

            Infos = new List<ItemInfo>();
            foreach (var info in obj.Infos)
            {
                Infos.Add(info);
            }
        }

        public static ProfileBackpack Ensure(ProfileBackpack obj)
        {
            if (obj == null)
            {
                obj = new ProfileBackpack();
            }

            if (obj.Items == null)
            {
                obj.Items = new List<ItemData>();
            }
            return obj;
        }

        public void AddItem(ItemData data, ItemInfoData itemInfo)
        {
            var item = GetItem(data.id);
            if (item == null)
            {
                Items.Add(data);
                Items.Sort();
            }
            else
            {
                item.count += data.count;
            }

            ItemInfo info = GetInfo(data.id);
            if (info == null)
            {
                info = new ItemInfo();
                info.itemID = data.id;
                info.infos = new List<ItemInfoData>();
                Infos.Add(info);
            }

            info.infos.Add(itemInfo);
            info.infos.Sort();
        }

        public void RemoveItem(string id, ItemInfoData itemInfo)
        {
            var item = GetItem(id);
            if (item == null || item.count < 1) { return; }

            item.count--;
            if (item.count == 0) { Items.Remove(item); }

            var info = GetInfo(id);
            if (info != null) { info.infos.Remove(itemInfo); }
        }

        public ItemData GetItem(string id)
        {
            foreach (var item in Items)
            {
                if (item.id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public ItemInfo GetInfo(string id)
        {
            foreach (var item in Infos)
            {
                if (item.itemID.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
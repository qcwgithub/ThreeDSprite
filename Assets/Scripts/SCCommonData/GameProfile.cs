using System;

namespace Data
{
    [Serializable]
    public class GameProfile
    {
        public string Version;
        public string UserID;

        public MapInfoProfile MapInfo;
        public ProfilePlayerInfo Info;
        public ProfileCastle Castle;
        public ProfileBackpack BackPack;

        public static GameProfile Ensure(GameProfile p)
        {
            if (p == null)
            {
                p = new GameProfile();
            }

            if (string.IsNullOrEmpty(p.Version))
            {
                p.Version = "1.0";
            }

            p.MapInfo = MapInfoProfile.Ensure(p.MapInfo);
            p.Info = ProfilePlayerInfo.Ensure(p.Info);
            p.Castle = ProfileCastle.Ensure(p.Castle);
            p.BackPack = ProfileBackpack.Ensure(p.BackPack);

            return p;
        }
    }
}
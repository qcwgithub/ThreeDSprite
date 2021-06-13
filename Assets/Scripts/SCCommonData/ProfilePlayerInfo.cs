using System;
using System.Numerics;

namespace Data
{
    [Serializable]
    public class ProfilePlayerInfo
    {
        public int KingdomLevel;
        public int KingdomEXP;
        public int Diamond;
        public BigInteger Money;
        public int Vitality;
        public BigInteger StrengthenMate;  // 强化材料
        public int AdvancedMate;    // 进阶材料
        public int LastTimeLeaveGame;
        public int LastTimeVitalityUpdate;
        public int LastTimeGetReward;

        public ProfilePlayerInfo() { }

        public ProfilePlayerInfo(ProfilePlayerInfo obj)
        {
            KingdomLevel = obj.KingdomLevel < 1 ? 1 : obj.KingdomLevel;
            KingdomEXP = obj.KingdomEXP;
            Diamond = obj.Diamond;
            Money = obj.Money;
            Vitality = obj.Vitality;
            StrengthenMate = obj.StrengthenMate;
            AdvancedMate = obj.AdvancedMate;
            LastTimeLeaveGame = obj.LastTimeLeaveGame;
            LastTimeGetReward = obj.LastTimeGetReward;
            LastTimeVitalityUpdate = obj.LastTimeVitalityUpdate;
        }

        public static ProfilePlayerInfo Ensure(ProfilePlayerInfo obj)
        {
            return obj;
        }
    }
}
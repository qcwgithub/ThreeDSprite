using System;
using System.Numerics;

namespace Data
{
    [Serializable]
    public class MaterialInfo
    {
        public BigInteger money;
        public int diamond;
        public BigInteger strengthenMate;   // 强化材料
        public int advancedMate;    // 进阶材料
        public int kingdomEXP;    // 王国经验

        public MaterialInfo()
        {
            money = 0;
            diamond = 0;
            strengthenMate = 0;
            advancedMate = 0;
            kingdomEXP = 0;
        }

        public bool HasReward()
        {
            return money > 0 || diamond > 0 || strengthenMate > 0 || advancedMate > 0 || kingdomEXP > 0;
        }
    }
}
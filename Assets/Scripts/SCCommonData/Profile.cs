using System;
using System.Numerics;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class Profile
    {
        #region Profile Auto

        [Key(0)]
        public int level;
        [Key(1)]
        public BigInteger money;
        [Key(2)]
        public int diamond;
        [Key(3)]
        public string portrait;
        [Key(4)]
        public string userName;
        [Key(5)]
        public int characterConfigId;

        #endregion Profile Auto

        public static Profile Ensure(Profile obj)
        {
            if (obj == null)
            {
                obj = new Profile();
            }

            if (obj.level <= 0)
            {
                obj.level = 1;
            }

            if (obj.characterConfigId <= 0)
            {
                obj.characterConfigId = 1;
            }

            return obj;
        }        
    }
}
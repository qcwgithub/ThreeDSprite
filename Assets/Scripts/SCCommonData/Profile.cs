using System;
using System.Numerics;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class Profile
    {
        [Key(0)]
        public int level;
        [Key(1)]
        public int money;
        [Key(2)]
        public int diamond;
        [Key(3)]
        public string portrait;
        [Key(4)]
        public string userName;        
    }
}
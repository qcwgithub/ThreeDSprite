using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public partial class BMPlayerInfo
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public int battleId;

        [IgnoreMember]
        public BMBattleInfo battle;
        [IgnoreMember]
        public btCharacter character;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public partial class BMPlayer
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public int battleId;
        [Key(2)]
        public int characterConfigId;

        [IgnoreMember]
        public BMBattle battle;
        [IgnoreMember]
        public btCharacter character;
    }
}
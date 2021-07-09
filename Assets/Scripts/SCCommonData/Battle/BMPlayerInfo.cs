using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Data
{
    public partial class BMPlayerInfo
    {
        public int playerId;
        public int battleId;

        [JsonIgnore]
        public BMBattleInfo battle;

        [JsonIgnore]
        public btCharacter character;
    }
}
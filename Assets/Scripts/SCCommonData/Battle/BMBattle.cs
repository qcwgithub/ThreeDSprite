using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public partial class BMBattle : btBattle
    {
        [Key(2)]
        public int battleId;
        [Key(3)]
        public Dictionary<int, BMPlayer> playerDict = new Dictionary<int, BMPlayer>();
        public BMPlayer GetPlayer(int playerId)
        {
            BMPlayer player;
            return this.playerDict.TryGetValue(playerId, out player) ? player : null;
        }
    }
}
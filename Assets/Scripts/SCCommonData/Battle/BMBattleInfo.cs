using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public partial class BMBattleInfo : btBattle
    {
        [Key(2)]
        public int battleId;
        [Key(3)]
        public Dictionary<int, BMPlayerInfo> playerDict = new Dictionary<int, BMPlayerInfo>();
        public BMPlayerInfo GetPlayer(int playerId)
        {
            BMPlayerInfo playerInfo;
            return this.playerDict.TryGetValue(playerId, out playerInfo) ? playerInfo : null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public partial class BMBattleInfo : btBattle
    {
        public int battleId;
        public Dictionary<int, BMPlayerInfo> playerDict = new Dictionary<int, BMPlayerInfo>();
        public BMPlayerInfo GetPlayer(int playerId)
        {
            BMPlayerInfo playerInfo;
            if (this.playerDict.TryGetValue(playerId, out playerInfo))
            {
                return playerInfo;
            }
            return null;
        }
    }
}
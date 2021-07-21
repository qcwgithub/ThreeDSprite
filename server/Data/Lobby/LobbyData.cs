using System.Collections.Generic;

namespace Data
{
    public sealed class LobbyData : ServerData
    {
        public int battleId = 0;

        public Dictionary<int, LobbyPlayer> playerDict = new Dictionary<int, LobbyPlayer>();
        public LobbyPlayer GetPlayer(int playerId)
        {
            LobbyPlayer player;
            return this.playerDict.TryGetValue(playerId, out player) ? player : null;
        }
        
        public Dictionary<int, LobbyBMInfo> bmInfos = new Dictionary<int, LobbyBMInfo>();
        public LobbyBMInfo GetBMInfo(int bmId)
        {
            LobbyBMInfo info;
            return this.bmInfos.TryGetValue(bmId, out info) ? info : null;
        }

        // public Dictionary<int, LobbyBattleInfo> battleInfos = new Dictionary<int, LobbyBattleInfo>();
        // public LobbyBattleInfo GetBattleInfo(int battleId)
        // {
        //     LobbyBattleInfo info;
        //     if (!this.battleInfos.TryGetValue(battleId, out info))
        //     {
        //         return null;
        //     }
        //     return info;
        // }

        public bool bmReady;
        public int bmReadyTimer;
    }
}
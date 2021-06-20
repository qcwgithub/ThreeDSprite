using System.Collections.Generic;

namespace Data
{
    public sealed class LobbyData : ServerData
    {
        public int battleId = 0;

        public Dictionary<int, LobbyPlayerInfo> playerInfos = new Dictionary<int, LobbyPlayerInfo>();
        public LobbyPlayerInfo GetPlayerInfo(int id)
        {
            LobbyPlayerInfo info;
            if (!this.playerInfos.TryGetValue(id, out info))
                return null;
            return info;
        }
        
        public Dictionary<int, LobbyBMInfo> bmInfos = new Dictionary<int, LobbyBMInfo>();
        public LobbyBMInfo GetBMInfo(int bmId)
        {
            LobbyBMInfo info;
            if (!this.bmInfos.TryGetValue(bmId, out info))
                return null;
            return info;
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
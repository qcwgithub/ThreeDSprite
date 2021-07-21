using System.Collections.Generic;

namespace Data
{
    public sealed class AAAData : ServerData
    {
        public bool active = true;  // false表示不接受客户端连接
        public bool playerIdReady = false;
        public bool pmReady = false;
        public int pmReadyTimer = 0;

        // account infos
        // accountInfos: Dictionary<string, AAAAccountInfo> = new Dictionary<string, AAAAccountInfo>();

        // player infos
        public Dictionary<int, AAAPlayer> playerDict = new Dictionary<int, AAAPlayer>();
        public AAAPlayer GetPlayer(int playerId)
        {
            AAAPlayer info;
            return this.playerDict.TryGetValue(playerId, out info) ? info : null;
        }

        // player manager
        public Dictionary<int, AAAPlayerManager> playerManagerDict = new Dictionary<int, AAAPlayerManager>();
        public AAAPlayerManager GetPlayerManager(int pmId)
        {
            AAAPlayerManager pm;
            return this.playerManagerDict.TryGetValue(pmId, out pm) ? pm : null;
        }

        // 0 means not ready
        public int nextPlayerId = 0;
    }
}
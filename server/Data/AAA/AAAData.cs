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
        public Dictionary<int, AAAPlayerInfo> playerInfos = new Dictionary<int, AAAPlayerInfo>();
        public AAAPlayerInfo GetPlayerInfo(int id)
        {
            AAAPlayerInfo info;
            return this.playerInfos.TryGetValue(id, out info) ? info : null;
        }

        // player manager info
        public Dictionary<int, AAAPlayerManagerInfo> playerManagerInfos = new Dictionary<int, AAAPlayerManagerInfo>();
        public AAAPlayerManagerInfo GetPlayerManagerInfo(int id)
        {
            AAAPlayerManagerInfo info;
            return this.playerManagerInfos.TryGetValue(id, out info) ? info : null;
        }

        // 0 means not ready
        public int nextPlayerId = 0;
    }
}
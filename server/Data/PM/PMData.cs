using System.Collections.Generic;

namespace Data
{
    public class _Active
    {
        public bool first;
        public int count;
        public bool requirePlayerList;
        public int timer;
    }

    public sealed class PMData : ServerBaseData, IGameConfigs
    {
        // playerId -> PlayerData
        public Dictionary<int, PMPlayerInfo> playerInfos = new Dictionary<int, PMPlayerInfo>();
        public PMPlayerInfo GetPlayerInfo(int id)
        {
            PMPlayerInfo info;
            if (!this.playerInfos.TryGetValue(id, out info))
            {
                return null;
            }
            return info;
        }

        public _Active alive = new _Active
        {
            first = true,
            count = 0,
            requirePlayerList = false,
            timer = -1,
        };

        public bool aaaReady = false;
        public bool allowNewPlayer = true; // false 表示 AAA 不会分配新玩家到此 PM（滚服）
        public bool allowClientConnect = true; // false 表示不接受客户端连接
                                               // playerDestroyTimeoutS 必须要 > playerSaveIntervalS
        public int playerDestroyTimeoutS = 600;  // 下线后多久清除此玩家
        public int playerSCSaveIntervalS = 60;

        // 配置文件
    }
}
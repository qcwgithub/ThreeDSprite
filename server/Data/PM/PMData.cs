using System.Collections.Generic;

namespace Data
{
    public class _PMActive
    {
        public bool first;
        public int count;
        public bool requirePlayerList;
    }

    public sealed class PMData : ServerData, IGameConfigs
    {
        // playerId -> PlayerData
        public Dictionary<int, PMPlayer> playerDict = new Dictionary<int, PMPlayer>();
        public PMPlayer GetPlayerInfo(int playerId)
        {
            PMPlayer player;
            return this.playerDict.TryGetValue(playerId, out player) ? player : null;
        }

        public _PMActive alive = new _PMActive
        {
            first = true,
            count = 0,
            requirePlayerList = false,
        };

        public bool aaaReady = false;
        public bool allowNewPlayer = true; // false 表示 AAA 不会分配新玩家到此 PM（滚服）
        public bool allowClientConnect = true; // false 表示不接受客户端连接
                                               // playerDestroyTimeoutS 必须要 > playerSaveIntervalS
        public int playerDestroyTimeoutS = 600;  // 下线后多久清除此玩家
        public int playerSCSaveIntervalS = 60;

        // 配置文件

        //// IGameConfigs
        public int minCharacterConfigId { get; set; }
        public int maxCharacterConfigId { get; set; }
        public Dictionary<int, CharacterConfig> characterConfigDict { get; set; }
        public CharacterConfig GetCharacterConfig(int characterId)
        {
            CharacterConfig characterConfig;
            return this.characterConfigDict.TryGetValue(characterId, out characterConfig) ? characterConfig : null;
        }
    }
}
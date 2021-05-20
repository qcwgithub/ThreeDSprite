using System.Collections.Generic;

public class AAAData {
    public bool active = true;  // false表示不接受客户端连接
    public bool playerIdReady = false;
    public bool pmReady = false;
    public int pmReadyTimer = -1;

    // account infos
    // accountInfos: Dictionary<string, AAAAccountInfo> = new Dictionary<string, AAAAccountInfo>();

    // player infos
    public Dictionary<int, AAAPlayerInfo> playerInfos = new Dictionary<int, AAAPlayerInfo>();
    public AAAPlayerInfo GetPlayerInfo(int id)
    {
        AAAPlayerInfo info;
        if (!this.playerInfos.TryGetValue(id, out info))
            return null;
        return info;
    }

    // player manager info
    public Dictionary<int, AAAPlayerManagerInfo> playerManagerInfos = new Dictionary<int, AAAPlayerManagerInfo>();
    public AAAPlayerManagerInfo GetAAAPlayerManagerInfo(int id)
    {
        AAAPlayerManagerInfo info;
        if (!this.playerManagerInfos.TryGetValue(id, out info))
            return null;
        return info;
    }

    // 0 means not ready
    public int nextPlayerId = 0;
}
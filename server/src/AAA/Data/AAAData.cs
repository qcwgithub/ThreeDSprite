
public class AAAData {
    public active = true;  // false表示不接受客户端连接
    public playerIdReady = false;
    public pmReady = false;
    public pmReadyTimer: NodeJS.Timeout = null;

    // account infos
    // accountInfos: Dictionary<string, AAAAccountInfo> = new Dictionary<string, AAAAccountInfo>();

    // player infos
    playerInfos: Dictionary<number, AAAPlayerInfo> = new Dictionary<number, AAAPlayerInfo>();

    // player manager info
    playerManagerInfos: Dictionary<number, AAAPlayerManagerInfo> = new Dictionary<number, AAAPlayerManagerInfo>();

    // 0 means not ready
    nextPlayerId = 0;
}
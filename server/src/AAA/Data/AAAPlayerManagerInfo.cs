public class AAAPlayerManagerInfo {
    int id; // player server id
    object socket;
    int playerCount;
    allowNewPlayer: boolean; // false 表示 AAA 不会分配新玩家到此 PM
}
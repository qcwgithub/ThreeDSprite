namespace Data
{
    public class AAAPlayerManagerInfo
    {
        public int id; // player server id
        public ISocket socket;
        public int playerCount;
        public bool allowNewPlayer; // false 表示 AAA 不会分配新玩家到此 PM
    }
}
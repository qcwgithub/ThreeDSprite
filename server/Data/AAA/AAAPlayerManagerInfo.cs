namespace Data
{
    public sealed class AAAPlayerManagerInfo
    {
        public int pmId;
        public int playerCount;
        public bool allowNewPlayer; // false 表示 AAA 不会分配新玩家到此 PM
    }
}
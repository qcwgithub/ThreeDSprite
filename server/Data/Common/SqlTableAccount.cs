using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class SqlTableAccount
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string platform;
        [Key(2)]
        public bool isBan;
        [Key(3)]
        public int unbanTimeS;          // DateTime
        [Key(4)]
        public string channel;
        [Key(5)]
        public string channelUserId;
        [Key(6)]
        public int createTimeS;         // DateTime
        [Key(7)]
        public string oaid;
        [Key(8)]
        public string imei;
        [Key(9)]
        public string userInfo;
    }
}
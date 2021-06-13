namespace Data
{
    public class SqlTableAccount : ISerializable
    {
        public int playerId;
        public string platform;
        public bool isBan;
        public int unbanTimeS;          // DateTime
        public string channel;
        public string channelUserId;
        public int createTimeS;         // DateTime
        public string oaid;
        public string imei;
        public string userInfo;
    }
}
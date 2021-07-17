namespace Data
{
    public static class MyChannels
    {
        public static readonly string debug = "debug";
        public static readonly string pc = "pc";
        public static readonly string uuid = "uuid";
        public static readonly string apple = "apple";
        public static readonly string leiting = "leiting";
        public static readonly string ivy = "ivy";

        public static bool isValidChannel(string channelType)
        {
            return channelType == MyChannels.debug ||
                channelType == MyChannels.pc ||
                channelType == MyChannels.uuid ||
                channelType == MyChannels.apple ||
                channelType == MyChannels.leiting ||
                channelType == MyChannels.ivy;
        }
    }
}
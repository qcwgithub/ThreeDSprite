using Data;

namespace Script
{
    public class AAAServer : Server
    {
        public override BaseData baseData { get { return this.aaaData; } }
        
        public AAAData aaaData;
        public AAAScript aaaScript;
        public AAASqlUtils aaaSqlUtils;
        public AAAChannel_Uuid channelUuid;
        public AAAChannel_Debug channelDebug;
        public AAAChannel_Apple channelApple;
        // public AAAChannel_Leiting channelLeiting;
        public AAAChannel_Ivy channelIvy;
    }
}

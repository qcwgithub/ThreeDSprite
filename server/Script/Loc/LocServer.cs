using Data;

namespace Script
{
    public class LocServer : Server
    {
        public override BaseData baseData { get { return this.locData; } }

        public LocData locData;
        public LocScript locScript;
    }
}
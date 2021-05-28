using Data;

namespace Script
{
    public class DBServer : Server
    {
        public override BaseData baseData { get { return this.dbData; } }

        public DBData dbData;
        public DBScript dbScript;
    }
}

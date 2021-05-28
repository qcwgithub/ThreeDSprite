using Data;

namespace Script
{
    public abstract class DBHandler : Handler<DBServer>
    {
        public DBData dbData { get { return this.server.dbData; } }
        public DBScript dbScript { get { return this.server.dbScript; } }
    }
}
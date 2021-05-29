using Data;

namespace Script
{
    public class DBServer : Server
    {
        public DBData dbData
        {
            get
            {
                return (DBData)this.baseData;
            }
        }
        public DBScript dbScript;

        public override void Create(DataEntry dataEntry, int id)
        {
            base.Create(dataEntry, id);
            base.AddHandler<DBServer>();

            this.dbScript = new DBScript { server = this };
            this.dispatcher.addHandler(new DBStart { server = this });
            this.dispatcher.addHandler(new DBQuery { server = this });
            this.dispatcher.addHandler(new DBTest { server = this });
            this.dispatcher.addHandler(new DBGetSummary { server = this });
        }
    }
}

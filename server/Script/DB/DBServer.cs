using Data;

namespace Script
{
    public class DBServer : Server
    {
        public DBData dbData
        {
            get
            {
                return (DBData)this.data;
            }
        }
        public DBScript dbScript;

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<DBServer>();

            this.dbScript = new DBScript { server = this };
            this.dispatcher.addHandler(new DBChangeChannel { server = this });
            this.dispatcher.addHandler(new DBGetSummary { server = this });

            this.dispatcher.addHandler(new DBInsertAccount { server = this });
            this.dispatcher.addHandler(new DBInsertPayiOS { server = this });
            this.dispatcher.addHandler(new DBInsertPlayer { server = this });

            this.dispatcher.addHandler(new DBLogChangeChannel { server = this });
            this.dispatcher.addHandler(new DBLogPlayerLogin { server = this });
            this.dispatcher.addHandler(new DBLogPlayerLogout { server = this });

            this.dispatcher.addHandler(new DBQueryAccountByChannel { server = this });
            this.dispatcher.addHandler(new DBQueryAccountByPlayerId { server = this });
            this.dispatcher.addHandler(new DBQueryAccountForChangeChannel { server = this });

            this.dispatcher.addHandler(new DBQueryPlayerById { server = this });
            this.dispatcher.addHandler(new DBQueryPlayerId { server = this });
            this.dispatcher.addHandler(new DBUpdatePlayerId { server = this });

            this.dispatcher.addHandler(new DBTest { server = this });
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}

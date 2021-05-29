using Data;

namespace Script
{
    public class LocServer : Server
    {
        public LocData locData
        {
            get
            {
                return (LocData)this.baseData;
            }
        }
        public LocScript locScript;

        public override void Create(DataEntry dataEntry, int id)
        {
            base.Create(dataEntry, id);
            base.AddHandler<LocServer>();

            this.locScript = new LocScript { server = this };
            this.dispatcher.addHandler(new LocStart { server = this });
            this.dispatcher.addHandler(new LocOnDisconnect { server = this });
            this.dispatcher.addHandler(new LocReportLoc { server = this });
            this.dispatcher.addHandler(new LocRequestLoc { server = this });
            this.dispatcher.addHandler(new LocBroadcast { server = this });
            this.dispatcher.addHandler(new LocGetSummary { server = this });
        }
    }
}
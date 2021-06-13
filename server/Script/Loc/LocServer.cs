using Data;

namespace Script
{
    public class LocServer : Server
    {
        public LocData locData
        {
            get
            {
                return (LocData)this.data;
            }
        }
        public LocScript locScript;

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<LocServer>();

            this.locScript = new LocScript { server = this };
            // this.dispatcher.addHandler(new LocStart { server = this });
            this.dispatcher.addHandler(new LocOnCLose { server = this });
            this.dispatcher.addHandler(new LocReportLoc { server = this });
            this.dispatcher.addHandler(new LocRequestLoc { server = this });
            this.dispatcher.addHandler(new LocBroadcast { server = this });
            this.dispatcher.addHandler(new LocGetSummary { server = this });
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
using Data;

namespace Script
{
    public class AAAServer : Server
    {
        public AAAData aaaData
        {
            get
            {
                return (AAAData)this.data;
            }
        }

        public AAAScript aaaScript;
        public AAASqlUtils aaaSqlUtils;
        public AAAChannel_Uuid channelUuid;
        public AAAChannel_Debug channelDebug;
        public AAAChannel_Apple channelApple;
        // public AAAChannel_Leiting channelLeiting;
        public AAAChannel_Ivy channelIvy;

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<AAAServer>();

            this.aaaScript = new AAAScript { server = this };
            this.aaaSqlUtils = new AAASqlUtils { server = this };
            this.channelUuid = new AAAChannel_Uuid { server = this };
            this.channelDebug = new AAAChannel_Debug { server = this };
            this.channelApple = new AAAChannel_Apple { server = this };
            this.channelIvy = new AAAChannel_Ivy { server = this };

            // this.dispatcher.addHandler(new AAAStart { server = this });
            this.dispatcher.addHandler(new AAATest { server = this });
            this.dispatcher.addHandler(new AAAOnPMAlive { server = this });
            this.dispatcher.addHandler(new AAASetPMReady { server = this });
            this.dispatcher.addHandler(new AAALoadPlayerId { server = this });
            this.dispatcher.addHandler(new AAAChangeChannel { server = this });
            this.dispatcher.addHandler(new AAAPlayerLogin { server = this });
            this.dispatcher.addHandler(new AAADestroyPlayer { server = this });
            this.dispatcher.addHandler(new AAAGetSummary { server = this });
            this.dispatcher.addHandler(new AAAShutdown { server = this });
            this.dispatcher.addHandler(new AAAAction { server = this });
        }

        public override void OnStart()
        {
            base.OnStart();
            
            this.setTimer(0, MsgType.AAALoadPlayerId, null);
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}

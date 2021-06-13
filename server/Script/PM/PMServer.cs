using Data;

namespace Script
{
    public class PMServer : Server, IGameScripts
    {
        public PMData pmData
        {
            get
            {
                return (PMData)this.data;
            }
        }
        public PMScript pmScript;
        public PMSqlUtils pmSqlUtils;
        public PMPlayerToSqlTablePlayer pmPlayerToSqlTablePlayer;
        public PMScriptCreateNewPlayer pmScriptCreateNewPlayer;

        public SCUtils scUtils { get; set; }
        public BattlefieldScript BattlefieldScript { get; set; }
        public GameScript gameScript;

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<PMServer>();

            this.pmScript = new PMScript { server = this };
            this.pmSqlUtils = new PMSqlUtils { server = this };
            this.pmPlayerToSqlTablePlayer = new PMPlayerToSqlTablePlayer { server = this };
            this.pmScriptCreateNewPlayer = new PMScriptCreateNewPlayer { server = this };

            this.gameScript = new GameScriptServer();
            this.gameScript.Init(this.pmData, this);

            this.scUtils = new SCUtils();
            this.scUtils.Init(this.pmData, this);

            this.BattlefieldScript = new BattlefieldScript();
            this.BattlefieldScript.Init(this.pmData, this);

            // this.dispatcher.addHandler(new PMStart { server = this });
            this.dispatcher.addHandler(new PMAction { server = this });
            this.dispatcher.addHandler(new PMChangeChannel { server = this });
            this.dispatcher.addHandler(new PMDestroyPlayer { server = this });
            this.dispatcher.addHandler(new PMKeepAliveToAAA { server = this });
            this.dispatcher.addHandler(new PMOnClose { server = this });
            this.dispatcher.addHandler(new PMPlayerLogin { server = this });
            this.dispatcher.addHandler(new PMPlayerSave { server = this });
            this.dispatcher.addHandler(new PMPreparePlayerLogin { server = this });
            this.dispatcher.addHandler(new PMSendDestroyPlayer { server = this });
        }

        public override void OnStart()
        {
            base.OnStart();
            
            this.setTimer(0, MsgType.PMKeepAliveToAAA, null);
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
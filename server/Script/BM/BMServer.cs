using Data;
using System.IO;

namespace Script
{
    public class BMServer : Server, IBattleScripts
    {
        public BMData bmData
        {
            get
            {
                return (BMData)this.data;
            }
        }

        public btMainScript mainScript { get; set; }
        public btMoveScript moveScript { get; set; }

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<BMServer>();
            
            this.dispatcher.addHandler(new BMKeepAliveToLobby { server = this });
            this.dispatcher.addHandler(new BMNewBattle { server = this });
            this.dispatcher.addHandler(new BMPlayerEnter { server = this });
            this.dispatcher.addHandler(new BMPlayerExit { server = this });

            this.dispatcher.addHandler(new BMMove { server = this });

            BattleScript.initBattleScripts(this.bmData, this);
        }

        public override void OnStart()
        {
            base.OnStart();
            
            this.setTimer(0, MsgType.BMKeepAliveToLobby, null);
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
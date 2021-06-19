using Data;
using System.IO;

namespace Script
{
    public class BMServer : Server
    {
        public BMData bmData
        {
            get
            {
                return (BMData)this.data;
            }
        }

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<BMServer>();
            
            this.dispatcher.addHandler(new BMKeepAliveToLobby { server = this });
            this.dispatcher.addHandler(new BMNewBattle { server = this });
            this.dispatcher.addHandler(new BMPlayerEnter { server = this });
            this.dispatcher.addHandler(new BMPlayerExit { server = this });
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
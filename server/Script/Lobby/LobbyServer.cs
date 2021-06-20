using Data;
using System.IO;

namespace Script
{
    public class LobbyServer : Server
    {
        public LobbyData lobbyData
        {
            get
            {
                return (LobbyData)this.data;
            }
        }

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<LobbyServer>();

            this.dispatcher.addHandler(new LobbyDestroyBattle { server = this });
            this.dispatcher.addHandler(new LobbyOnBMAlive { server = this });
            this.dispatcher.addHandler(new LobbyPlayerEnterBattle { server = this });
            this.dispatcher.addHandler(new LobbyPlayerExitBattle { server = this });
            this.dispatcher.addHandler(new LobbySetBMReady { server = this });
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
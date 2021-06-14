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
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
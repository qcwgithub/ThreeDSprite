using Data;

namespace Script
{
    public class PMScriptCreateNewPlayer : IServerScript<PMServer>
    {
        public PMServer server { get; set; }
        
        public log4net.ILog logger { get { return this.server.logger; } }
        public PMData pmData { get { return this.server.pmData; } }

        public PMPlayer newPlayer(int playerId, string channel, string channelUserId, string userName)
        {
            var player = new PMPlayer();
            player.playerId = playerId;
            return player;
        }
    }
}
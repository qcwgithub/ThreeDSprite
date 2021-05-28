using Data;

namespace Script
{
    public class PMScriptCreateNewPlayer : IScript<PMServer>
    {
        public PMServer server { get; set; }
        
        public log4net.ILog logger { get { return this.server.logger; } }
        public PMData pmData { get { return this.server.pmData; } }

        public PMPlayerInfo newPlayer(int playerId, string channel, string channelUserId, string userName)
        {
            var player = new PMPlayerInfo();
            player.id = playerId;
            return player;
        }
    }
}
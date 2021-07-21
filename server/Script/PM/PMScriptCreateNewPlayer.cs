using Data;

namespace Script
{
    public class PMScriptCreateNewPlayer : IServerScript<PMServer>
    {
        public PMServer server { get; set; }
        
        public log4net.ILog logger { get { return this.server.logger; } }
        public PMData pmData { get { return this.server.pmData; } }

        public PMPlayer NewPlayer(int playerId, string channel, string channelUserId, string userName)
        {
            var player = new PMPlayer();
            player.playerId = playerId;
            Profile profile = player.profile = new Profile();

            #region PMScriptCreateNewPlayer Auto

            profile.level = 1;
            profile.money = 0;
            profile.diamond = 0;
            profile.portrait = "";
            profile.userName = "";
            profile.characterConfigId = 1;

            #endregion PMScriptCreateNewPlayer Auto
            
            player.profile = Profile.Ensure(profile);
            return player;
        }
    }
}
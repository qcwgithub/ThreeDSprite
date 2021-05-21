
public class PMScriptCreateNewPlayer : IScript
{
    public Server server { get; set; }
    public FakeLogger logger { get { return this.server.logger; } }
    public PMData pmData { get { return this.server.pmData; } }

    public PMPlayerInfo newPlayer(int playerId, string channel, string channelUserId, string userName) {
        var player = new PMPlayerInfo();
        player.id = playerId;
        player.server = this.server;

        var defaultProfile = this.server.JSON.parse(this.server.pmData.defaultProfileConfig) as CProfile;
        if (channel == HermesChannels.uuid) {
            defaultProfile.userID = channelUserId;
        }
        else {
            defaultProfile.userID = v4();
        }

        if (userName == null || !(userName.Length > 0)) {
            userName = "Master_" + this.server.scUtils.randomString(9);
        }
        player.profile = CProfile.ensure(defaultProfile, userName);
        return player;
    }
}

public class PMScriptCreateNewPlayer : IScript
{
    public Server server { get; set; }
    public log4net.ILog logger { get { return this.server.logger; } }
    public PMData pmData { get { return this.server.pmData; } }

    public PMPlayerInfo newPlayer(int playerId, string channel, string channelUserId, string userName)
    {
        var player = new PMPlayerInfo();
        player.id = playerId;
        player.server = this.server;
        return player;
    }
}
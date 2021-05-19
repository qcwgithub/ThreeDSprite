
public class PMPlayerToSqlTablePlayer : IScript {
    public Server server { get; set; }

    public SqlTablePlayer convert(PMPlayerInfo player) {
        var tb = new SqlTablePlayer();
        tb.id = player.id;

        var p = player.profile;
        return tb;
    }
}
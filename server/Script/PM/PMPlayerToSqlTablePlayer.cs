using Data;

namespace Script
{
    public class PMPlayerToSqlTablePlayer : IScript<PMServer>
    {
        public PMServer server { get; set; }

        public SqlTablePlayer convert(PMPlayerInfo player)
        {
            var tb = new SqlTablePlayer();
            tb.id = player.id;

            var p = player.profile;
            return tb;
        }
    }
}
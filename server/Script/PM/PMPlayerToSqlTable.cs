using Data;
using System.Numerics;

namespace Script
{
    public class PMPlayerToSqlTable : IServerScript<PMServer>
    {
        public PMServer server { get; set; }

        public SqlTablePlayer Convert(PMPlayer player)
        {
            var sql = new SqlTablePlayer();
            sql.playerId = player.playerId;

            var profile = player.profile;

            #region PMPlayerToSqlTable Auto

            sql.level = profile.level;
            sql.money = profile.money;
            sql.diamond = profile.diamond;
            sql.portrait = profile.portrait;
            sql.userName = profile.userName;
            sql.characterConfigId = profile.characterConfigId;

            #endregion PMPlayerToSqlTable Auto

            return sql;
        }
    }
}
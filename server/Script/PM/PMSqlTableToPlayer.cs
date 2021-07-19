using Data;
using System.Numerics;

namespace Script
{
    public class PMSqlTableToPlayer : IServerScript<PMServer>
    {
        public PMServer server { get; set; }
        
        public PMPlayer DecodePlayer(SqlTablePlayer sql)
        {
            var player = new PMPlayer();
            player.id = sql.id;
            var profile = player.profile = new Profile();

            #region PMSqlTableToPlayer Auto

            profile.level = sql.level;
            profile.money = BigInteger.Parse(sql.money);
            profile.diamond = sql.diamond;
            profile.portrait = sql.portrait;
            profile.userName = sql.userName;
            profile.characterConfigId = sql.characterConfigId;

            #endregion PMSqlTableToPlayer Auto

            profile = Profile.Ensure(profile);
            return player;
        }
    }
}
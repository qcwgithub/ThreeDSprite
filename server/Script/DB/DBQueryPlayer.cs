using Data;
using System.Numerics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public abstract class DBQueryPlayer : DBQuery
    {
        protected async Task DecodeSqlTablePlayer(MySqlDataReader reader, List<SqlTablePlayer> list)
        {
            var helper = new MySqlDataReaderHelper(this.server, reader);
            while (await helper.ReadRow())
            {
                var table = new SqlTablePlayer();
                table.playerId = helper.GetInt32("playerId");
                
                #region DBQueryPlayer Auto

                table.level = helper.GetInt32("level");
                table.money = BigInteger.Parse(helper.GetString("money"));
                table.diamond = helper.GetInt32("diamond");
                table.portrait = helper.GetString("portrait");
                table.userName = helper.GetString("userName");
                table.characterConfigId = helper.GetInt32("characterConfigId");

                #endregion DBQueryPlayer Auto
                
                list.Add(table);
            }
        }
    }
}
using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public abstract class DBQueryPlayer : DBQuery
    {
        protected async Task decodeSqlTablePlayer(MySqlDataReader reader, List<SqlTablePlayer> list)
        {
            var helper = new MySqlDataReaderHelper(this.server, reader);
            while (await helper.ReadRow())
            {
                var table = new SqlTablePlayer();
                table.playerId = helper.GetInt32("id");
                // ..........
                list.Add(table);
            }
        }
    }
}
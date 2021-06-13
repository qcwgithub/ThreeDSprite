using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public abstract class DBQueryAccount : DBQuery
    {
        protected async Task decodeSqlTableAccount(MySqlDataReader reader, List<SqlTableAccount> list)
        {
            var helper = new MySqlDataReaderHelper(this.server, reader);
            while (await helper.ReadRow())
            {
                var table = new SqlTableAccount();
                table.playerId = helper.GetInt32("playerId");
                table.platform = helper.GetString("platform");
                table.isBan = helper.GetBoolean("isBan");
                table.unbanTimeS = helper.GetDateTimeToSeconds("unbanTime");
                table.channel = helper.GetString("channel");
                table.channelUserId = helper.GetString("channelUserId");
                table.createTimeS = helper.GetDateTimeToSeconds("createTime");
                table.oaid = helper.GetString("oaid");
                table.imei = helper.GetString("imei");
                table.userInfo = helper.GetString("userInfo");
                list.Add(table);
            }
        }
    }
}
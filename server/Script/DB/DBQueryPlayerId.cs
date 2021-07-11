using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBQueryPlayerId : DBQuery
    {
        public override MsgType msgType { get { return MsgType.DBQueryPlayerId; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg/* null */)
        {
            string queryStr = "SELECT playerId FROM player_id";
            MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, queryStr, null);
            var helper = new MySqlDataReaderHelper(this.server, reader);
            if (!(await helper.ReadRow()))
            {
                return ECode.Error;
            }
            int playerId = helper.GetInt32("playerId");
            await reader.CloseAsync();

            return new MyResponse(ECode.Success, new ResDBQueryAccountPlayerId { playerId = playerId });
        }
    }
}
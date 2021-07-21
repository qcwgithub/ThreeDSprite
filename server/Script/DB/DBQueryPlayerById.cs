using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBQueryPlayerById : DBQueryPlayer
    {
        public override MsgType msgType => MsgType.DBQueryPlayerById;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgQueryPlayerById>(_msg);
            string queryStr = "SELECT * FROM player WHERE playerId=@0;";
            MySqlParameter[] param = this.MakeParameters(new List<object> { msg.playerId});

            MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, queryStr, param);
            var res = new ResQueryPlayer();
            res.list = new List<SqlTablePlayer>();
            await this.DecodeSqlTablePlayer(reader, res.list);
            await reader.CloseAsync();

            return new MyResponse(ECode.Success, res);
        }
    }
}
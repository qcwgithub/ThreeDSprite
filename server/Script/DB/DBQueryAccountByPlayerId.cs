using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBQueryAccountByPlayerId : DBQueryAccount
    {
        public override MsgType msgType => MsgType.DBQueryAccountByPlayerId;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgQueryAccountByPlayerId>(_msg);
            var queryStr = "SELECT * FROM account WHERE playerId=@0;";
            var param = this.MakeParameters(msg.playerId);

            MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, queryStr, param);
            var res = new ResQueryAccount();
            res.list = new List<SqlTableAccount>();
            await this.decodeSqlTableAccount(reader, res.list);
            await reader.CloseAsync();

            return new MyResponse(ECode.Success, res);
        }
    }
}
using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBQueryAccountByChannel : DBQueryAccount
    {
        public override MsgType msgType => MsgType.DBQueryAccountByChannel;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgQueryAccountByChannel>(_msg);
            string queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1";
            MySqlParameter[] param = this.MakeParameters(msg.channel, msg.channelUserId);

            MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, queryStr, param);
            var res = new ResQueryAccount();
            res.list = new List<SqlTableAccount>();
            await this.decodeSqlTableAccount(reader, res.list);
            await reader.CloseAsync();

            return new MyResponse(ECode.Success, res);
        }
    }
}
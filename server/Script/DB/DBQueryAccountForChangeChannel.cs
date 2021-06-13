using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBQueryAccountForChangeChannel : DBQueryAccount
    {
        public override MsgType msgType => MsgType.DBQueryAccountForChangeChannel;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgQueryAccountForChangeChannel>(_msg);
            string queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1 AND NOT EXISTS(SELECT * FROM account WHERE channel=@2 AND channelUserId=@3);";
            MySqlParameter[] param = this.makeParameters(msg.channel, msg.channelUserId, msg.notExistChannel, msg.notExistChannelUserId);

            MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, queryStr, param);
            var res = new ResQueryAccount();
            res.list = new List<SqlTableAccount>();
            await this.decodeSqlTableAccount(reader, res.list);
            await reader.CloseAsync();

            return new MyResponse(ECode.Success, res);
        }
    }
}
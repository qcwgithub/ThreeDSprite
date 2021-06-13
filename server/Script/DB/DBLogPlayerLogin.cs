using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBLogPlayerLogin : DBQuery
    {
        public override MsgType msgType => MsgType.DBLogPlayerLogin;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgLogPlayerLogin>(_msg);

            var queryStr = "INSERT INTO player_login (playerId) VALUES (@0);";
            var param = this.makeParameters(msg.playerId);

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}
using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBLogPlayerLogout : DBQuery
    {
        public override MsgType msgType => MsgType.DBLogPlayerLogout;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgLogPlayerLogout>(_msg);

            var queryStr = "INSERT INTO player_logout (playerId,level) VALUES (@0,1);";
            var param = this.makeParameters(msg.playerId);

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}
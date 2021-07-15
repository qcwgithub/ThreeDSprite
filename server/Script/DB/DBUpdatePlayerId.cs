using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBUpdatePlayerId : DBQuery
    {
        public override MsgType msgType => MsgType.DBUpdatePlayerId;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var playerId = this.server.castObject<MsgQueryAccountUpdatePlayerId>(_msg).playerId;
            var queryStr = "UPDATE player_id SET playerId = @0";
            var param = new MySqlParameter[] { new MySqlParameter("@0", playerId) };

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}